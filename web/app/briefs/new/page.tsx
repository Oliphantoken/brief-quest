"use client";
import { useState, FormEvent } from "react";
import { useRouter } from "next/navigation";
import { createBrief } from "@/app/lib/api";

export default function NewBriefPage() {
  const router = useRouter();
  const [title, setTitle] = useState("");
  const [text, setText] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e: React.SubmitEvent<HTMLFormElement>) {
    e.preventDefault();
    setLoading(true);
    setError(null);
    try {
      const brief = await createBrief(title, text);
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
      <form onSubmit={handleSubmit} className="flex flex-col gap-4">
        <input type="text" placeholder="Title" value={title}
          onChange={e => setTitle(e.target.value)} required
          className="border rounded px-3 py-2" />
        <textarea placeholder="Paste the full brief text here"
          value={text} onChange={e => setText(e.target.value)} required
          rows={12} className="border rounded px-3 py-2 font-mono text-sm" />
        {error && <p className="text-red-600 text-sm">{error}</p>}
        <button type="submit" disabled={loading}
          className="bg-teal-600 text-white rounded px-4 py-2 disabled:opacity-50">
          {loading ? "Saving..." : "Save brief"}
        </button>
      </form>
    </main>
  );
}