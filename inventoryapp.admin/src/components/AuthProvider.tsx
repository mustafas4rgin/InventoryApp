"use client";

import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";

export default function AuthProvider({ children }: { children: React.ReactNode }) {
  const router = useRouter();
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem("accessToken");
    const userRole = localStorage.getItem("userRole");
    if (!token || userRole !== "Admin") {
      router.push("/unauthorized");
    } else {
      setLoading(false); // token varsa loading false yap
    }
  }, [router]);

  if (loading) {
    return <div>Loading...</div>; // veya boş bir şey render edebilirsin
  }

  return <>{children}</>;
}
