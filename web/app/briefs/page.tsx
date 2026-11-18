"use client";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { getBriefs } from "@/app/lib/api";
import { getToken, clearToken } from "@/app/lib/token";
import Link from "next/link";

interface Brief { id: number; title: string; text: string; }

export default function BriefsPage() {
  const router = useRouter();
  const [briefs, setBriefs] = useState<Brief[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    setMounted(true);
  }, []);

  useEffect(() => {
    if (!mounted) return;
    if (!getToken()) {
      router.replace("/auth");
      return;
    }
    getBriefs()
      .then(setBriefs)
      .catch(e => {
        if(e.message !== "Session expired.") setError(e.message);
      })
      .finally(() => setLoading(false));
}, [mounted]);

  function signOut() {
    clearToken();
    router.push("/auth");
  }

  if (loading) return (
    <main className="p-8">
      <p className="text-gray-500 animate-pulse">Loading brief...</p>
      <p className="text-xs text-gray-400 mt-2">
        First load may take a moment while the server wakes up.
      </p>
    </main>
  );
  

  return (
    <main className="max-w-2xl mx-auto mt-10 p-4">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold">My briefs</h1>
        <div className="flex gap-3">
          <Link href="/briefs/new"
            className="bg-teal-600 text-white px-4 py-2 rounded">
            New brief
          </Link>
          <button onClick={signOut} className="text-sm text-gray-500 underline">
            Sign out
          </button>
        </div>
      </div>
      {error && <p className="text-red-600">{error}</p>}

      {!loading && briefs.length === 0 && !error && (
        <p className="text-gray-500">No briefs yet. Create one to get started.</p>
      )}
      
      <ul className="flex flex-col gap-3">
        {briefs.map(b => (
          <li key={b.id}>
            <Link href={`/briefs/${b.id}`}
              className="block border rounded-lg p-4 hover:bg-gray-50">
              <p className="font-semibold">{b.title}</p>
              <p className="text-sm text-gray-500 mt-1 line-clamp-2">{b.text}</p>
            </Link>
          </li>
        ))}
      </ul>
    </main>
  );
}