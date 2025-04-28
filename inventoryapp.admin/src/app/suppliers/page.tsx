"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import axiosInstance from "@/lib/axiosInstance";
import AuthProvider from "@/components/AuthProvider";
import Sidebar from "@/components/Sidebar";
import { toast } from "react-hot-toast";
import { Pencil, Trash2, X } from "lucide-react";
import * as Dialog from "@radix-ui/react-dialog";

interface Supplier {
  id: number;
  name: string;
  users: User[];
}

interface User {
  id: number;
  firstName: string;
  lastName: string;
}

export default function SuppliersPage() {
  const [suppliers, setSuppliers] = useState<Supplier[]>([]);
  const [searchTerm, setSearchTerm] = useState("");
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage] = useState(5);

  const [open, setOpen] = useState(false);
  const [name, setName] = useState("");
  const [editingSupplierId, setEditingSupplierId] = useState<number | null>(null);

  const fetchSuppliers = async () => {
    const res = await axiosInstance.get("/Supplier/GetAll?include=users");
    setSuppliers(res.data);
  };

  useEffect(() => {
    fetchSuppliers();
  }, []);

  const handleCreateOrUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (editingSupplierId) {
        await axiosInstance.put(`/Supplier/Update/${editingSupplierId}`, { name });
        toast.success("Supplier updated successfully!");
      } else {
        await axiosInstance.post("/Supplier/Add", { name });
        toast.success("Supplier created successfully!");
      }
      setName("");
      setOpen(false);
      setEditingSupplierId(null);
      fetchSuppliers();
    } catch (error) {
      console.error(error);
      toast.error("Failed to save supplier.");
    }
  };

  const handleDelete = async (id: number) => {
    if (confirm("Are you sure you want to delete?")) {
      try {
        await axiosInstance.delete(`/Supplier/${id}`);
        toast.success("Supplier deleted successfully!");
        fetchSuppliers();
      } catch (error) {
        console.error(error);
        toast.error("Failed to delete supplier.");
      }
    }
  };

  const filteredSuppliers = suppliers.filter((supplier) =>
    supplier.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const totalPages = Math.ceil(filteredSuppliers.length / itemsPerPage);
  const indexOfLastItem = currentPage * itemsPerPage;
  const indexOfFirstItem = indexOfLastItem - itemsPerPage;
  const currentSuppliers = filteredSuppliers.slice(indexOfFirstItem, indexOfLastItem);

  return (
    <AuthProvider>
      <div className="flex min-h-screen bg-gray-100">
        <Sidebar />
        <div className="flex-1 p-8 space-y-8 animate-fadeIn">
          <div className="flex justify-between items-center">
            <h1 className="text-3xl font-bold text-gray-800">Suppliers</h1>
            <Dialog.Root open={open} onOpenChange={setOpen}>
              <Dialog.Trigger asChild>
                <button className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg font-semibold shadow-md transition-all">
                  {editingSupplierId ? "Edit Supplier" : "+ Add Supplier"}
                </button>
              </Dialog.Trigger>
              <Dialog.Portal>
                <Dialog.Overlay className="fixed inset-0 bg-black/30 backdrop-blur-sm" />
                <Dialog.Content className="fixed top-1/2 left-1/2 w-full max-w-md -translate-x-1/2 -translate-y-1/2 bg-white p-6 rounded-xl shadow-xl space-y-6 animate-scaleIn">
                  <div className="flex justify-between items-center">
                    <Dialog.Title className="text-2xl font-bold text-gray-800">
                      {editingSupplierId ? "Edit Supplier" : "Add Supplier"}
                    </Dialog.Title>
                    <Dialog.Close asChild>
                      <button className="text-gray-400 hover:text-gray-600">
                        <X size={24} />
                      </button>
                    </Dialog.Close>
                  </div>
                  <form onSubmit={handleCreateOrUpdate} className="flex flex-col gap-4">
                    <input
                      type="text"
                      placeholder="Supplier Name"
                      value={name}
                      onChange={(e) => setName(e.target.value)}
                      className="p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                      required
                    />
                    <button
                      type="submit"
                      className="bg-green-600 hover:bg-green-700 text-white py-3 rounded-lg font-semibold transition-all duration-300"
                    >
                      Save
                    </button>
                  </form>
                </Dialog.Content>
              </Dialog.Portal>
            </Dialog.Root>
          </div>

          <div className="flex justify-between items-center">
            <input
              type="text"
              placeholder="Search suppliers..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full max-w-sm p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          <div className="overflow-x-auto bg-white rounded-lg shadow-md animate-fadeIn">
            <table className="min-w-full">
              <thead className="bg-gray-200">
                <tr>
                  <th className="py-3 px-6 text-left text-sm font-semibold">Name</th>
                  <th className="py-3 px-6 text-left text-sm font-semibold">First User</th>
                  <th className="py-3 px-6 text-center text-sm font-semibold">Actions</th>
                </tr>
              </thead>
              <tbody>
                {currentSuppliers.length === 0 ? (
                  <tr>
                    <td colSpan={3} className="text-center py-10 text-gray-500">
                      No suppliers found.
                    </td>
                  </tr>
                ) : (
                  currentSuppliers.map((supplier) => (
                    <tr key={supplier.id} className="border-t hover:bg-gray-50 transition-all">
                      <td className="py-3 px-6">
                        <Link href={`/suppliers/${supplier.id}`} className="text-blue-600 hover:underline">
                          {supplier.name}
                        </Link>
                      </td>
                      <td className="py-3 px-6">
                        {supplier.users.length > 0
                          ? `${supplier.users[0].firstName} ${supplier.users[0].lastName}`
                          : "-"}
                      </td>
                      <td className="py-3 px-6 flex justify-center gap-2">
                        <button
                          onClick={() => {
                            setName(supplier.name);
                            setEditingSupplierId(supplier.id);
                            setOpen(true);
                          }}
                          className="p-2 bg-yellow-400 hover:bg-yellow-500 text-white rounded-lg transition"
                        >
                          <Pencil size={18} />
                        </button>
                        <button
                          onClick={() => handleDelete(supplier.id)}
                          className="p-2 bg-red-500 hover:bg-red-600 text-white rounded-lg transition"
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

          {totalPages > 1 && (
            <div className="flex justify-center items-center gap-2">
              {Array.from({ length: totalPages }, (_, index) => index + 1).map((page) => (
                <button
                  key={page}
                  onClick={() => setCurrentPage(page)}
                  className={`px-4 py-2 rounded-lg text-sm font-semibold ${page === currentPage
                    ? "bg-blue-600 text-white"
                    : "bg-white border border-gray-300 text-gray-700 hover:bg-gray-100"
                    } transition-all`}
                >
                  {page}
                </button>
              ))}
            </div>
          )}
        </div>
      </div>
    </AuthProvider>
  );
}
