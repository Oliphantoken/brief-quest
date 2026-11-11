"use client";
import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { getToken } from "@/app/lib/token";

export default function Home() {
  const router = useRouter();
  useEffect(() => {
    router.replace(getToken() ? "/briefs" : "/auth");
  }, [router]);
  return null;
}