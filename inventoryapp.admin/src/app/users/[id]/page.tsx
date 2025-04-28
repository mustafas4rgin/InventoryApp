"use client";

import { useEffect, useState } from "react";
import { useParams } from "next/navigation";
import axiosInstance from "@/lib/axiosInstance";
import AuthProvider from "@/components/AuthProvider";
import Sidebar from "@/components/Sidebar";
import Link from "next/link";
import { toast } from "react-hot-toast";

interface User {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  roleName: string;
  supplierName: string;
  createdAt: string;
  updatedAt: string;
}

export default function UserDetailPage() {
  const { id } = useParams();
  const [user, setUser] = useState<User | null>(null);

  const fetchUser = async () => {
    try {
      const res = await axiosInstance.get(`/User/${id}?include=role,supplier`);
      setUser(res.data);
    } catch (error) {
      console.error(error);
      toast.error("Failed to fetch user.");
    }
  };

  useEffect(() => {
    if (id) {
      fetchUser();
    }
  }, [id]);

  if (!user) {
    return <div className="p-8">Loading...</div>;
  }

  return (
    <AuthProvider>
      <div className="flex min-h-screen bg-gray-100">
        <Sidebar />
        <div className="flex-1 p-8 space-y-6">
          <h1 className="text-3xl font-bold text-gray-800">
            {user.firstName} {user.lastName}
          </h1>

          <div className="space-y-2 text-gray-700">
            <p><strong>Email:</strong> {user.email}</p>
            <p><strong>Role:</strong> {user.roleName}</p>
            <p><strong>Supplier:</strong> {user.supplierName}</p>
            <p><strong>Created At:</strong> {user.createdAt}</p>
            <p><strong>Updated At:</strong> {user.updatedAt}</p>
          </div>

          <Link
            href="/users"
            className="inline-block mt-6 text-blue-600 hover:underline"
          >
            ← Back to users
          </Link>
        </div>
      </div>
    </AuthProvider>
  );
}
