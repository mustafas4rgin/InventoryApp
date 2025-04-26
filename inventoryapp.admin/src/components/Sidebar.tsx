"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";

export default function Sidebar() {
  const router = useRouter();
  const [role, setRole] = useState<string | null>(null);

  useEffect(() => {
    const storedRole = localStorage.getItem("userRole");
    setRole(storedRole);
  }, []);

  const handleLogout = () => {
    localStorage.clear();
    document.cookie = "accessToken=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
    router.push("/login");
  };

  return (
    <div className="w-64 h-screen bg-white border-r flex flex-col p-6 shadow-md">
      <h1 className="text-2xl font-bold mb-10 text-blue-600 text-center">InventoryApp</h1>

      {role && (
        <nav className="flex flex-col gap-4 text-gray-700">
          <Link href="/dashboard" className="hover:text-blue-600 font-medium transition">
            🏠 Dashboard
          </Link>

          <Link href="/products" className="hover:text-blue-600 font-medium transition">
            📦 Products
          </Link>


              <Link href="/categories" className="hover:text-blue-600 font-medium transition">
                📁 Categories
              </Link>
              <Link href="/suppliers" className="hover:text-blue-600 font-medium transition">
                🚚 Suppliers
              </Link>
              <Link href="/users" className="hover:text-blue-600 font-medium transition">
                👤 Users
              </Link>
              <Link href="/roles" className="hover:text-blue-600 font-medium transition">
                🛡️ Roles
              </Link>
        </nav>
      )}

      <div className="mt-auto">
        <button
          onClick={handleLogout}
          className="w-full mt-8 bg-red-500 hover:bg-red-600 text-white py-2 rounded-lg font-semibold transition"
        >
          Logout
        </button>
      </div>
    </div>
  );
}
