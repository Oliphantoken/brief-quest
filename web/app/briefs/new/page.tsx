"use client";
import { useState, FormEvent } from "react";
import { useRouter } from "next/navigation";
import { createBrief, apiFetch } from "@/app/lib/api";

const API = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5000";

export default function NewBriefPage() {
  const router = useRouter();
  const [mode, setMode] = useState<"text" | "file">("text");
  const [title, setTitle] = useState("");
  const [text, setText] = useState("");
  const [file, setFile] = useState<File | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

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
        if (!res.ok) {
          const msg = await res.text();
          throw new Error(`Extraction failed: ${msg}`);
        }
      }

      router.push(`/briefs/${brief.id}`);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : "Something went wrong.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <main className="max-w-2xl mx-auto mt-10 p-4">
      <h1 className="text-2xl font-bold mb-6">New brief</h1>

      <div className="flex gap-4 mb-6">
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
          Upload PDF or Word
        </button>
      </div>

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

        {error && <p className="text-red-600 text-sm">{error}</p>}
        <button type="submit" disabled={loading || (mode === "file" && !file)}
          className="bg-teal-600 text-white rounded px-4 py-2 disabled:opacity-50">
          {loading
            ? mode === "file" ? "Uploading and extracting..." : "Saving..."
            : mode === "file" ? "Upload and extract" : "Save brief"}
        </button>
      </form>
    </main>
  );
}