"use client";

import { useEffect, useState } from "react";
import { useParams } from "next/navigation";
import axiosInstance from "@/lib/axiosInstance";
import AuthProvider from "@/components/AuthProvider";
import Sidebar from "@/components/Sidebar";
import Link from "next/link";
import { toast } from "react-hot-toast";

interface Supplier {
  id: number;
  name: string;
  users: {
    id: number;
    firstName: string;
    lastName: string;
  }[];
}

export default function SupplierDetailPage() {
  const { id } = useParams();
  const [supplier, setSupplier] = useState<Supplier | null>(null);

  const fetchSupplier = async () => {
    try {
      const res = await axiosInstance.get(`/Supplier/${id}?include=users`);
      setSupplier(res.data);
    } catch (error) {
      console.error(error);
      toast.error("Failed to fetch supplier.");
    }
  };

  useEffect(() => {
    if (id) {
      fetchSupplier();
    }
  }, [id]);

  if (!supplier) {
    return <div className="p-8">Loading...</div>;
  }

  return (
    <AuthProvider>
      <div className="flex min-h-screen bg-gray-100">
        <Sidebar />
        <div className="flex-1 p-8 space-y-6">
          <h1 className="text-3xl font-bold text-gray-800">{supplier.name}</h1>

          <h2 className="text-xl font-semibold text-gray-700">Users</h2>

          {supplier.users.length === 0 ? (
            <p className="text-gray-500">No users found.</p>
          ) : (
            <ul className="list-disc list-inside space-y-2">
              {supplier.users.map((user) => (
                <li key={user.id}>
                  <Link
                    href={`/users/${user.id}`}
                    className="text-blue-600 hover:underline"
                  >
                    {user.firstName} {user.lastName}
                  </Link>
                </li>
              ))}
            </ul>
          )}

          <Link
            href="/suppliers"
            className="inline-block mt-6 text-blue-600 hover:underline"
          >
            ← Back to suppliers
          </Link>
        </div>
      </div>
    </AuthProvider>
  );
}
