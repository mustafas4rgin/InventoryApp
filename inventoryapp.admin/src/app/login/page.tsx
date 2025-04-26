"use client"
import { setCookie } from "nookies";
import axiosInstance from "@/lib/axiosInstance";
import { useRouter } from "next/navigation";
import { useState } from "react";

export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const router = useRouter();

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const res = await axiosInstance.post("/Auth/login", {
        email,
        password,
      });

      const accessToken = res.data.accessToken;
      const refreshToken = res.data.refreshToken;

      if (accessToken && refreshToken) {
        // 1. Önce /auth/me'den user bilgilerini çek
        const meResponse = await axiosInstance.get("/Auth/me", {
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        });
        const userRole = meResponse.data.role;

        // 2. Eğer user Admin değilse, hiçbir şey kaydetmeden unauthorized'a at
        if (userRole !== "Admin") {
          console.log("Unauthorized.");
          await router.push("/unauthorized");
          return; // burada function'ı bitiriyoruz
        }

        // 3. Eğer user Admin ise tokenları ve role'u kaydet
        localStorage.setItem("accessToken", accessToken);
        localStorage.setItem("refreshToken", refreshToken);
        localStorage.setItem("userRole", userRole);

        setCookie(null, "accessToken", accessToken, {
          path: "/",
          maxAge: 60 * 60 * 24,
        });

        setCookie(null, "userRole", userRole, {
          path: "/",
          maxAge: 60 * 60 * 24,
        });

        // 4. Sonunda dashboard'a yönlendir
        await router.push("/dashboard");
      } else {
        alert("Login failed!");
      }
    } catch (error) {
      console.error(error);
      alert("Login failed!");
    }
  };

  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-100">
      <form onSubmit={handleLogin} className="bg-white p-8 rounded shadow-md w-96">
        <h2 className="text-2xl mb-6 text-center font-bold">Login</h2>
        <input
          type="email"
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          className="w-full p-2 mb-4 border border-black-300 rounded"
          required
        />
        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          className="w-full p-2 mb-6 border border-black-300 rounded"
          required
        />
        <button
          type="submit"
          className="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700"
        >
          Login
        </button>
      </form>
    </div>
  );
}
