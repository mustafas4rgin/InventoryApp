"use client";

import { useEffect, useState } from "react";
import axiosInstance from "@/lib/axiosInstance";
import AuthProvider from "@/components/AuthProvider";
import Sidebar from "@/components/Sidebar";
import { toast } from "react-hot-toast";
import { Trash2 } from "lucide-react"; // silme ikonu

interface Role {
  id: number;
  name: string;
}

interface User {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  createdAt: string;
  updatedAt: string;
  roleName: string;
}

export default function UsersPage() {
  const [users, setUsers] = useState<User[]>([]);
  const [roles, setRoles] = useState<Role[]>([]);
  const [loading, setLoading] = useState(false);

  const fetchUsersAndRoles = async () => {
    try {
      const [usersRes, rolesRes] = await Promise.all([
        axiosInstance.get("/User/GetAll?include=role"),
        axiosInstance.get("/Role/GetAll"),
      ]);
      setUsers(usersRes.data);
      setRoles(rolesRes.data);
    } catch (error) {
      console.error(error);
      toast.error("Failed to fetch users or roles.");
    }
  };

  useEffect(() => {
    fetchUsersAndRoles();
  }, []);

  const handleRoleChange = async (userId: number, newRoleId: number) => {
    setLoading(true);
    try {
      await axiosInstance.put(`/User/${userId}/Role/${newRoleId}`, { roleId: newRoleId });
      toast.success("Role updated successfully!");
      fetchUsersAndRoles();
    } catch (error) {
      console.error(error);
      toast.error("Failed to update role.");
    }
    setLoading(false);
  };

  const handleDeleteUser = async (userId: number) => {
    if (!confirm("Are you sure you want to delete this user?")) return;
    setLoading(true);
    try {
      await axiosInstance.delete(`/users/${userId}`);
      toast.success("User deleted successfully!");
      fetchUsersAndRoles();
    } catch (error) {
      console.error(error);
      toast.error("Failed to delete user.");
    }
    setLoading(false);
  };

  return (
    <AuthProvider>
      <div className="flex min-h-screen bg-gray-100">
        <Sidebar />
        <div className="flex-1 p-8 space-y-8">
          <h1 className="text-3xl font-bold text-gray-800">Users</h1>

          <div className="overflow-x-auto bg-white rounded-lg shadow-md">
            <table className="min-w-full">
              <thead className="bg-gray-200">
                <tr>
                  <th className="py-3 px-6 text-left text-sm font-semibold">Name</th>
                  <th className="py-3 px-6 text-left text-sm font-semibold">Email</th>
                  <th className="py-3 px-6 text-left text-sm font-semibold">Current Role</th>
                  <th className="py-3 px-6 text-center text-sm font-semibold">Change Role</th>
                  <th className="py-3 px-6 text-center text-sm font-semibold">Actions</th>
                </tr>
              </thead>
              <tbody>
                {users.length === 0 ? (
                  <tr>
                    <td colSpan={5} className="text-center py-10 text-gray-500">
                      No users found.
                    </td>
                  </tr>
                ) : (
                  users.map((user) => (
                    <tr key={user.id} className="border-t hover:bg-gray-50 transition">
                      <td className="py-3 px-6">{user.firstName} {user.lastName}</td>
                      <td className="py-3 px-6">{user.email}</td>
                      <td className="py-3 px-6">{user.roleName}</td>
                      <td className="py-3 px-6 text-center">
                        <select
                          onChange={(e) => handleRoleChange(user.id, Number(e.target.value))}
                          className="p-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-sm"
                          disabled={loading}
                          defaultValue=""
                        >
                          <option value="" disabled>
                            Select new role
                          </option>
                          {roles.map((role) => (
                            <option key={role.id} value={role.id}>
                              {role.name}
                            </option>
                          ))}
                        </select>
                      </td>
                      <td className="py-3 px-6 flex justify-center">
                        <button
                          onClick={() => handleDeleteUser(user.id)}
                          className="p-2 bg-red-500 hover:bg-red-600 text-white rounded-lg transition"
                          disabled={loading}
                        >
                          <Trash2 size={18} />
                        </button>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </AuthProvider>
  );
}
