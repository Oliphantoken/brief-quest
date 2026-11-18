"use client";
import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { createBrief, apiFetch } from "@/app/lib/api";
import { getToken, clearToken } from "@/app/lib/token";
import Link from "next/link";


const API = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5000";


export default function NewBriefPage() {
  const router = useRouter();
  const [mode, setMode] = useState<"text" | "file">("text");
  const [title, setTitle] = useState("");
  const [text, setText] = useState("");
  const [file, setFile] = useState<File | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
     setMounted(true);
   }, []);

  useEffect(() => {
    if (!mounted) return;
    if (typeof window !== "undefined" && !getToken()) { router.replace("/auth"); return; }
  }, [mounted]); //empty dependency array so the function runs once on mount only
  

  async function handleSubmit(e: React.SubmitEvent<HTMLFormElement>) {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      // 1. Create the brief with title and either the text or a placeholder
      const briefText = mode === "text" ? text : `Uploaded file: ${file?.name}`;
      const brief = await createBrief(title, briefText);

      // 2. If a file was chosen, extract from the file immediately
      if (mode === "file" && file) {
        const form = new FormData();
        form.append("file", file);
        const res = await apiFetch(`/briefs/${brief.id}/extract-file`, {
          method: "POST",
          body: form,
          // Don't set Content-Type: browser sets it with the boundary automatically
          headers: {},
        });
      }

    router.push(`/briefs/${brief.id}`);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : "Something went wrong.");
    } finally {
      setLoading(false);
    }
  }

  if (loading) return <p className="p-8 text-gray-500">Loading...</p>

  return (
    <main className="max-w-2xl mx-auto mt-10 p-4">
      <Link href="/briefs" className="text-sm text-teal-700 underline mb-4 block">
        ← Back to briefs
      </Link>
      <h1 className="text-2xl font-bold mb-2">New brief</h1>
      <p className="text-sm font-normal mb-6">Coming soon: upload your .docx or .pdf brief file!</p>

      {/* <div className="flex gap-4 mb-6">
        <button onClick={() => setMode("text")}
          className={`px-4 py-2 rounded border ${mode === "text"
            ? "bg-teal-600 text-white border-teal-600"
            : "text-teal-700 border-teal-600"}`}>
          Paste text
        </button>
        <button onClick={() => setMode("file")}
          className={`px-4 py-2 rounded border ${mode === "file"
            ? "bg-teal-600 text-white border-teal-600"
            : "text-teal-700 border-teal-600"}`}>
          Soon: Upload a PDF or Word (.docx) document
        </button>
      </div> */}

      <form onSubmit={handleSubmit} className="flex flex-col gap-4">
        <input type="text" placeholder="Title" value={title}
          onChange={e => setTitle(e.target.value)} required
          className="border rounded px-3 py-2" />

        {mode === "text" ? (
          <textarea placeholder="Paste the full brief text here"
            value={text} onChange={e => setText(e.target.value)} required
            rows={12} className="border rounded px-3 py-2 font-mono text-sm" />
        ) : (
          <div className="border-2 border-dashed border-teal-300 rounded-lg p-6 text-center">
            <input type="file" accept=".pdf,.docx"
              onChange={e => setFile(e.target.files?.[0] ?? null)}
              className="hidden" id="file-input" required />
            <label htmlFor="file-input" className="cursor-pointer">
              {file ? (
                <span className="text-teal-700 font-medium">{file.name}</span>
              ) : (
                <span className="text-gray-500">
                  Click to choose a PDF or Word (.docx) file
                </span>
              )}
            </label>
            {file && (
              <p className="text-xs text-gray-400 mt-2">
                {(file.size / 1024).toFixed(0)} KB
              </p>
            )}
          </div>
        )}

        {error && <p className="text-orange-400 text-sm">{error}</p>}

        <button type="submit" disabled={loading || (mode === "file" && !file)}
          className="bg-teal-600 text-white rounded px-4 py-2 disabled:opacity-50">
          {loading
            ? mode === "file" ? "Uploading and extracting..." : "Saving..."
            : mode === "file" ? "Upload and extract" : "Save brief"}
        </button>

         {/* Note on AI usage */}
        <p className="text-ms font-normal mb-2"><b>Note:</b>Google's Gemini AI is being used for brief extraction.</p>
        <p className="text-ms font-normal mb-2">Remove any sensitive or personal data <b>before</b> saving the brief.</p>

      </form>
    </main>
  );
}