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
  const briefID = Number(id);

  useEffect(() => {
    if(!Number.isInteger(briefID)) return;

    if (!getToken()) { router.replace("/auth"); return; }
    getBriefDetail(Number(id))
      .then(setBrief)
      .catch(e => setError(e.message));
  }, [briefID, router]);

  async function handleExtract() {
    setExtracting(true);
    setError(null);
    try {
      await extractBrief(Number(id));
      const updated = await getBriefDetail(Number(id));
      setBrief(updated);
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : "Extraction failed.");
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

  if (!brief) return <p className="p-8 text-gray-500">Loading...</p>;

  const tierOrder = ["Pass", "Merit", "Distinction", null];
  const sorted = [...brief.quests].sort(
    (a, b) => tierOrder.indexOf(a.tier) - tierOrder.indexOf(b.tier));

  return (
    <main className="max-w-2xl mx-auto mt-10 p-4">
      <Link href="/briefs" className="text-sm text-teal-700 underline mb-4 block">
        ← Back to briefs
      </Link>
      <h1 className="text-2xl font-bold mb-2">{brief.title}</h1>
      <p className="text-gray-500 text-sm mb-6 whitespace-pre-wrap">{brief.text}</p>

      {!brief.extractedAt && (
        <button onClick={handleExtract} disabled={extracting}
          className="bg-teal-600 text-white rounded px-4 py-2 mb-6 disabled:opacity-50">
          {extracting ? "Extracting..." : "Extract quests with AI"}
        </button>
      )}

      {error && <p className="text-red-600 mb-4">{error}</p>}

      {brief.quests.length > 0 && (
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