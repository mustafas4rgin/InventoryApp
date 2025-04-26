"use client";

import { useEffect, useState } from "react";
import axiosInstance from "@/lib/axiosInstance";
import AuthProvider from "@/components/AuthProvider";
import Sidebar from "@/components/Sidebar";
import { toast } from "react-hot-toast";
import * as Dialog from "@radix-ui/react-dialog";
import { X, Pencil, Trash2 } from "lucide-react";

interface Role {
  id: number;
  name: string;
}

export default function RolesPage() {
  const [roles, setRoles] = useState<Role[]>([]);
  const [openAdd, setOpenAdd] = useState(false);
  const [openEdit, setOpenEdit] = useState(false);
  const [name, setName] = useState("");
  const [selectedRole, setSelectedRole] = useState<Role | null>(null);

  const fetchRoles = async () => {
    try {
      const res = await axiosInstance.get("/Role/GetAll");
      setRoles(res.data);
    } catch (error) {
      console.error(error);
      toast.error("Failed to fetch roles.");
    }
  };

  useEffect(() => {
    fetchRoles();
  }, []);

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await axiosInstance.post("/Role/Add", { name });
      toast.success("Role created successfully!");
      setName("");
      setOpenAdd(false);
      fetchRoles();
    } catch (error) {
      console.error(error);
      toast.error("Failed to create role.");
    }
  };

  const handleUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedRole) return;
    try {
      await axiosInstance.put(`/Role/Update/${selectedRole.id}`, { name });
      toast.success("Role updated successfully!");
      setName("");
      setSelectedRole(null);
      setOpenEdit(false);
      fetchRoles();
    } catch (error) {
      console.error(error);
      toast.error("Failed to update role.");
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm("Are you sure you want to delete this role?")) return;
    try {
      await axiosInstance.delete(`/Role/${id}`);
      toast.success("Role deleted successfully!");
      fetchRoles();
    } catch (error) {
      console.error(error);
      toast.error("Failed to delete role.");
    }
  };

  return (
    <AuthProvider>
      <div className="flex min-h-screen bg-gray-100">
        <Sidebar />
        <div className="flex-1 p-8 space-y-8">
          <div className="flex justify-between items-center">
            <h1 className="text-3xl font-bold text-gray-800">Roles</h1>

            {/* Add Role Modal */}
            <Dialog.Root open={openAdd} onOpenChange={setOpenAdd}>
              <Dialog.Trigger asChild>
                <button className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg font-semibold shadow-md transition-all">
                  + Add Role
                </button>
              </Dialog.Trigger>

              <Dialog.Portal>
                <Dialog.Overlay className="fixed inset-0 bg-black/40 backdrop-blur-sm transition-all" />
                <Dialog.Content className="fixed top-1/2 left-1/2 w-full max-w-md -translate-x-1/2 -translate-y-1/2 bg-white p-6 rounded-xl shadow-lg space-y-6 animate-fadeIn">
                  <div className="flex justify-between items-center">
                    <Dialog.Title className="text-2xl font-bold text-gray-800">Add Role</Dialog.Title>
                    <Dialog.Close asChild>
                      <button className="text-gray-400 hover:text-gray-600 transition">
                        <X size={24} />
                      </button>
                    </Dialog.Close>
                  </div>

                  <form onSubmit={handleCreate} className="flex flex-col gap-4">
                    <input
                      type="text"
                      placeholder="Role Name"
                      value={name}
                      onChange={(e) => setName(e.target.value)}
                      className="p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                      required
                    />
                    <button
                      type="submit"
                      className="bg-green-600 hover:bg-green-700 text-white py-3 rounded-lg font-semibold transition-all duration-300"
                    >
                      Save Role
                    </button>
                  </form>
                </Dialog.Content>
              </Dialog.Portal>
            </Dialog.Root>
          </div>

          {/* Roles List */}
          <div className="overflow-x-auto bg-white rounded-lg shadow-md">
            <table className="min-w-full">
              <thead className="bg-gray-200">
                <tr>
                  <th className="py-3 px-6 text-left text-sm font-semibold">Role Name</th>
                  <th className="py-3 px-6 text-center text-sm font-semibold">Actions</th>
                </tr>
              </thead>
              <tbody>
                {roles.length === 0 ? (
                  <tr>
                    <td colSpan={2} className="text-center py-10 text-gray-500">
                      No roles found.
                    </td>
                  </tr>
                ) : (
                  roles.map((role) => (
                    <tr key={role.id} className="border-t hover:bg-gray-50 transition">
                      <td className="py-3 px-6">{role.name}</td>
                      <td className="py-3 px-6 flex justify-center gap-4">
                        {/* Edit Button */}
                        <button
                          onClick={() => {
                            setSelectedRole(role);
                            setName(role.name);
                            setOpenEdit(true);
                          }}
                          className="p-2 bg-yellow-400 hover:bg-yellow-500 text-white rounded-lg transition"
                        >
                          <Pencil size={16} />
                        </button>

                        {/* Delete Button */}
                        <button
                          onClick={() => handleDelete(role.id)}
                          className="p-2 bg-red-500 hover:bg-red-600 text-white rounded-lg transition"
                        >
                          <Trash2 size={16} />
                        </button>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>

          {/* Edit Role Modal */}
          <Dialog.Root open={openEdit} onOpenChange={setOpenEdit}>
            <Dialog.Portal>
              <Dialog.Overlay className="fixed inset-0 bg-black/40 backdrop-blur-sm transition-all" />
              <Dialog.Content className="fixed top-1/2 left-1/2 w-full max-w-md -translate-x-1/2 -translate-y-1/2 bg-white p-6 rounded-xl shadow-lg space-y-6 animate-fadeIn">
                <div className="flex justify-between items-center">
                  <Dialog.Title className="text-2xl font-bold text-gray-800">Edit Role</Dialog.Title>
                  <Dialog.Close asChild>
                    <button className="text-gray-400 hover:text-gray-600 transition">
                      <X size={24} />
                    </button>
                  </Dialog.Close>
                </div>

                <form onSubmit={handleUpdate} className="flex flex-col gap-4">
                  <input
                    type="text"
                    placeholder="Role Name"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    className="p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                    required
                  />
                  <button
                    type="submit"
                    className="bg-green-600 hover:bg-green-700 text-white py-3 rounded-lg font-semibold transition-all duration-300"
                  >
                    Update Role
                  </button>
                </form>
              </Dialog.Content>
            </Dialog.Portal>
          </Dialog.Root>
        </div>
      </div>
    </AuthProvider>
  );
}
