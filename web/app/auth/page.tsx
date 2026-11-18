"use client";
import { useState } from "react";
import { useRouter } from "next/navigation";
import { login, register } from "@/app/lib/api";
import { setToken } from "@/app/lib/token";

export default function AuthPage() {
  const router = useRouter();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [mode, setMode] = useState<"login" | "register">("login");

  async function handleSubmit(e: React.SubmitEvent) {
    e.preventDefault();
    setError(null);
    try {
      if (mode === "register") {
        await register(email, password);
        setMode("login");
        return;
      }
      const token = await login(email, password);
      setToken(token);
      router.push("/briefs");
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : "Something went wrong.");
    }
  }

  return (
    <main className="max-w-sm mx-auto mt-20 p-6 border rounded-xl">

      <h1 className="text-2xl font-bold mb-6">
        {mode === "login" ? "Sign in" : "Register"}
      </h1>
      <form onSubmit={handleSubmit} className="flex flex-col gap-4">
        <input type="email" placeholder="Email" value={email}
          onChange={e => setEmail(e.target.value)} required
          className="border rounded px-3 py-2" />
        <input type="password" placeholder="Password" value={password}
          onChange={e => setPassword(e.target.value)} required
          className="border rounded px-3 py-2" />
        
        <button type="submit" className="bg-teal-600 text-white rounded px-4 py-2">
          {mode === "login" ? "Sign in" : "Create account"}
        </button>
      </form>
      <button onClick={() => setMode(mode === "login" ? "register" : "login")}
        className="mt-4 text-sm text-teal-700 underline">
        {mode === "login" ? "No account? Register" : "Have an account? Sign in"}
      </button>
      
      {error && <p className="text-orange-400 text-sm">{error}</p>}

      <p className="text-ms text-gray-400 mt-2">
        Hello! This is a demo app, using free hosting services, so 
        please be patient, first load may take a moment while the server wakes up. 
        Thank you!
      </p>

    </main>
  );
}