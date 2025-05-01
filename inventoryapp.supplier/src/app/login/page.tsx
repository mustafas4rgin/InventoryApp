"use client";

import { useState } from "react";
import axiosInstance from "@/lib/axiosinstance";
import { setCookie } from "nookies";
import { useRouter } from "next/navigation";
import { toast } from "react-hot-toast";

export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const router = useRouter();

  const handleLogin = async () => {
    try {
      // 1. Önce login isteği at
      const loginRes = await axiosInstance.post("/auth/login", { email, password });
      const { accessToken } = loginRes.data;

      if (!accessToken) {
        toast.error("Access token not received.");
        return;
      }

      // 2. AccessToken'ı cookie'ye kaydet (1 saatlik)
      setCookie(null, "accessToken", accessToken, {
        path: "/",
        maxAge: 60 * 60, // 1 saat
      });

      // 3. Şimdi Auth/me'ye istek atıp kullanıcı bilgisi al
      const meRes = await axiosInstance.get("/auth/me", {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });
      const role = meRes.data.user.roleName;

      if (role !== "Supplier") {
        toast.error("Only suppliers can login here.");
        return;
      }

      // 4. Rol Supplier ise userRole'u cookie'ye yaz
      setCookie(null, "userRole", role, {
        path: "/",
        maxAge: 60 * 60, // 1 saat
      });

      toast.success("Login successful!");
      router.push("/dashboard");

    } catch (error) {
      console.error(error);
      toast.error("Login failed.");
    }
  };

  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-100">
      <div className="bg-white p-8 rounded shadow-md w-full max-w-md">
        <h1 className="text-2xl font-bold mb-6 text-center">Supplier Login</h1>
        <div className="space-y-4">
          <input
            type="email"
            placeholder="Email"
            className="w-full px-4 py-2 border rounded-lg"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
          <input
            type="password"
            placeholder="Password"
            className="w-full px-4 py-2 border rounded-lg"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
          <button
            onClick={handleLogin}
            className="w-full bg-blue-600 hover:bg-blue-700 text-white py-2 rounded-lg"
          >
            Login
          </button>
        </div>
      </div>
    </div>
  );
}
