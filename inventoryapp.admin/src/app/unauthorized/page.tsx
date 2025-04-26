"use client";

import { useRouter } from "next/navigation";

export default function UnauthorizedPage() {
  const router = useRouter();

  const handleLoginRedirect = () => {
    router.push("/login");
  };

  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-gray-100">
      <div className="bg-white p-8 rounded shadow-md text-center">
        <h1 className="text-3xl font-bold text-red-600 mb-4">Unauthorized</h1>
        <p className="text-gray-700 mb-6">Bu sayfaya erişim yetkiniz bulunmamaktadır.</p>
        <button
          onClick={handleLoginRedirect}
          className="px-6 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
        >
          Tekrar Giriş Yap
        </button>
      </div>
    </div>
  );
}
