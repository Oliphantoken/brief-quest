"use client";
import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { getBriefDetail, extractBrief, patchQuestStatus } from "@/app/lib/api";
import { getToken } from "@/app/lib/token";
import Link from "next/link";

interface Quest {
  id: number; text: string; sourceSentence: string;
  tier: string | null; status: string; firstStep: string | null;
}
interface BriefDetail {
  id: number; title: string; text: string;
  extractedAt: string | null; quests: Quest[];
}

export default function BriefPageClient() {
  const { id } = useParams<{ id: string }>();
  const router = useRouter();
  const [brief, setBrief] = useState<BriefDetail | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [extracting, setExtracting] = useState(false);
  const [loading, setLoading] = useState(true);
  const [mounted, setMounted] = useState(false);
  const briefID = Number(id);

 useEffect(() => {
     setMounted(true);
   }, []);

  useEffect(() => {
    if (!mounted) return;
    if(!Number.isInteger(briefID)) return;

    if (!getToken()) { router.replace("/auth"); return; }
    getBriefDetail(Number(id))
      .then(setBrief)
      .catch(e => {
        if(e.message !== "Session expired.") setError(e.message)
      })
      .finally(() => setLoading(false));
  }, [mounted]);

  //Handles messages for each status
 async function handleExtract() {
  setExtracting(true);
  setError(null);
  try {
    await extractBrief(Number(id));
    const updated = await getBriefDetail(Number(id));
    setBrief(updated);
  } catch (e: unknown) {
    const msg = e instanceof Error ? e.message : "Extraction failed.";
    if (msg.includes("503") || msg.includes("unavailable"))
      setError("The AI service is busy right now. Please try again in a minute.");
    else if (msg.includes("429") || msg.includes("limit"))
      setError("You have reached today's AI limit. Please try again tomorrow.");
    else if (msg.includes("409"))
      setError("This brief has already been extracted.");
    else
      setError("Extraction failed. Please try again.");
  } finally {
    setExtracting(false);
  }
}

  async function handleStatusChange(questId: number, status: string) {
    try {
      await patchQuestStatus(Number(id), questId, status);
      setBrief(prev => prev ? {
        ...prev,
        quests: prev.quests.map(q => q.id === questId ? { ...q, status } : q)
      } : prev);
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : "Could not update status.");
    }
  }

  //Loading brief
  if(loading || !brief) return (
    <main className="p-8">
      <p className="text-gray-500 animate-pulse">Loading brief...</p>
    </main>
  );

  const tierOrder = ["Pass", "Merit", "Distinction", null];
  const sorted = [...brief.quests].sort(
    (a, b) => tierOrder.indexOf(a.tier) - tierOrder.indexOf(b.tier));

  if(error) (
  <main className="p-8">
    <p className="text-red-600 mb-4">{error}</p>
    <Link href="/briefs" className="text-teal-700 underline text-sm">
      ← Back to briefs
    </Link>
  </main>
  );


  function CollapsibleText({ text }: { text: string }) {
    const [expanded, setExpanded] = useState(false);
    const PREVIEW_LINES = 4;

    // Split by line breaks, show only the first few lines when collapsed
    const lines = text.split("\n");
    const isLong = lines.length > PREVIEW_LINES || text.length > 400;
    const preview = lines.slice(0, PREVIEW_LINES).join("\n");

    return (
      <div className="mb-6">
        <p className={`whitespace-pre-wrap text-gray-600 text-sm ${
          !expanded && isLong ? "line-clamp-4" : ""
        }`}>
          {expanded || !isLong ? text : preview}
        </p>
        {isLong && (
          <button
            onClick={() => setExpanded(e => !e)}
            className="mt-1 text-xs text-teal-600 underline focus:outline-none">
            {expanded ? "Show less" : "Show more"}
          </button>
        )}
      </div>
    );
  }


  return (
    <main className="max-w-2xl mx-auto mt-10 p-4">
      <Link href="/briefs" className="text-sm text-teal-700 underline mb-4 block">
        ← Back to briefs
      </Link>

      {/* Brief Title */}
      <h1 className="text-2xl font-bold mb-2">{brief.title}</h1>

      {/* Collapsible brief text */}
      <CollapsibleText text={brief.text} />


      {/* Extract Button */
      !brief.extractedAt && (
        <button onClick={handleExtract} disabled={extracting}
          className="bg-teal-600 text-white rounded px-4 py-2 mb-6 disabled:opacity-50">
          {extracting ? "Extracting..." : "Extract quests with AI"}
        </button>
      )}

      {/* Progress bar */
      brief.quests.length > 0 && (() => {
        const done = brief.quests.filter(q => q.status === "Done").length;
        const total = brief.quests.length;
        const pct = Math.round((done / total) * 100);
        const tierCounts: Record<string, { total: number; done: number }> = {};

        for (const q of brief.quests) {
          const key = q.tier ?? "General";
          if (!tierCounts[key]) tierCounts[key] = { total: 0, done: 0 };
          tierCounts[key].total++;
          if (q.status === "Done") tierCounts[key].done++;
        }

        const tierColour: Record<string, string> = {
          Pass: "bg-blue-400",
          Merit: "bg-amber-400",
          Distinction: "bg-purple-400",
          General: "bg-gray-400",
        };

        return (
          <div className="mb-6">
            <div className="flex justify-between items-baseline mb-1">
              <h2 className="text-lg font-semibold">Progress</h2>
              <span className="text-sm text-gray-500">{done} / {total} done ({pct}%)</span>
            </div>

            {/* Overall bar */}
            <div className="w-full h-4 bg-gray-100 rounded-full overflow-hidden mb-3"
                role="progressbar" aria-valuenow={pct} aria-valuemin={0} aria-valuemax={100}
                aria-label={`Overall progress: ${pct}%`}>
              <div className="h-full bg-teal-500 rounded-full transition-all duration-300"
                  style={{ width: `${pct}%` }} />
            </div>

            {/* Per-tier breakdown */}
            <div className="flex flex-col gap-1.5">
              {["Pass", "Merit", "Distinction", "General"]
                .filter(t => tierCounts[t])
                .map(t => {
                  const { total: tt, done: td } = tierCounts[t];
                  const tpct = Math.round((td / tt) * 100);
                  return (
                    <div key={t} className="flex items-center gap-2">
                      <span className="text-xs w-20 text-gray-600 shrink-0">{t}</span>
                      <div className="flex-1 h-2 bg-gray-100 rounded-full overflow-hidden">
                        <div className={`h-full ${tierColour[t]} rounded-full transition-all duration-300`}
                            style={{ width: `${tpct}%` }} />
                      </div>
                      <span className="text-xs text-gray-400 w-12 text-right shrink-0">
                        {td}/{tt}
                      </span>
                    </div>
                  );
                })}
            </div>
          </div>
        );
      })()}

      {/* Quest list */
      brief.quests.length > 0 && (
        <>
          <h2 className="text-lg font-semibold mb-3">
            Quests ({brief.quests.filter(q => q.status === "Done").length}
            /{brief.quests.length} done)
          </h2>
          <ul className="flex flex-col gap-3">
            {sorted.map(q => (
              <li key={q.id}
                className="border rounded-lg p-4 flex gap-3 items-start">
                <input type="checkbox"
                  checked={q.status === "Done"}
                  onChange={e =>
                    handleStatusChange(q.id, e.target.checked ? "Done" : "Todo")}
                    aria-label={`Mark "${q.text}" as ${q.status === "Done" ? "not done" : "done"}`} // for screen readers
                  className="mt-1 accent-teal-600 w-5 h-5 flex-none" />
                <div className="flex-1">
                  <p className={q.status === "Done"
                    ? "line-through text-gray-400" : ""}>{q.text}</p>
                  {q.tier && (
                    <span className="text-xs font-medium mt-1 inline-block
                      px-2 py-0.5 rounded bg-teal-50 text-teal-700">
                      {q.tier}
                    </span>
                  )}
                  <p className="text-xs text-gray-400 mt-1 italic">
                    "{q.sourceSentence}"
                  </p>
                </div>
              </li>
            ))}
          </ul>
        </>
      )}
      
    </main>
  );
}