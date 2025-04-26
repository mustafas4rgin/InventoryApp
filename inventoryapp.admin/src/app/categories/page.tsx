"use client";

import { useEffect, useState } from "react";
import axiosInstance from "@/lib/axiosInstance";
import AuthProvider from "@/components/AuthProvider";
import Sidebar from "@/components/Sidebar";
import { toast } from "react-hot-toast";
import * as Dialog from "@radix-ui/react-dialog";
import { X } from "lucide-react";

interface Category {
  id: number;
  name: string;
}

export default function CategoriesPage() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [searchTerm, setSearchTerm] = useState("");
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage] = useState(5);

  const [open, setOpen] = useState(false);
  const [name, setName] = useState("");

  const fetchCategories = async () => {
    const res = await axiosInstance.get("/Category/GetAll");
    setCategories(res.data);
  };

  useEffect(() => {
    fetchCategories();
  }, []);

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await axiosInstance.post("/Category/Add", { name });
      toast.success("Category created successfully!");
      setName("");
      setOpen(false);
      fetchCategories();
    } catch (error) {
      console.error(error);
      toast.error("Failed to create category.");
    }
  };

  const handleDelete = async (id: number) => {
    if (confirm("Are you sure you want to delete?")) {
      try {
        await axiosInstance.delete(`/categories/${id}`);
        toast.success("Category deleted successfully!");
        fetchCategories();
      } catch (error) {
        console.error(error);
        toast.error("Failed to delete category.");
      }
    }
  };

  const filteredCategories = categories.filter((category) =>
    category.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const totalPages = Math.ceil(filteredCategories.length / itemsPerPage);
  const indexOfLastItem = currentPage * itemsPerPage;
  const indexOfFirstItem = indexOfLastItem - itemsPerPage;
  const currentCategories = filteredCategories.slice(indexOfFirstItem, indexOfLastItem);

  return (
    <AuthProvider>
      <div className="flex min-h-screen bg-gray-100">
        <Sidebar />
        <div className="flex-1 p-8 space-y-8">
          <div className="flex justify-between items-center">
            <h1 className="text-3xl font-bold text-gray-800">Categories</h1>
            {/* Add Category Modal */}
            <Dialog.Root open={open} onOpenChange={setOpen}>
              <Dialog.Trigger asChild>
                <button className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg font-semibold shadow-md transition-all">
                  + Add Category
                </button>
              </Dialog.Trigger>

              <Dialog.Portal>
                <Dialog.Overlay className="fixed inset-0 bg-black/40 backdrop-blur-sm transition-all" />
                <Dialog.Content className="fixed top-1/2 left-1/2 w-full max-w-md -translate-x-1/2 -translate-y-1/2 bg-white p-6 rounded-xl shadow-lg space-y-6 animate-fadeIn">
                  <div className="flex justify-between items-center">
                    <Dialog.Title className="text-2xl font-bold text-gray-800">Add Category</Dialog.Title>
                    <Dialog.Close asChild>
                      <button className="text-gray-400 hover:text-gray-600 transition">
                        <X size={24} />
                      </button>
                    </Dialog.Close>
                  </div>

                  <form onSubmit={handleCreate} className="flex flex-col gap-4">
                    <input
                      type="text"
                      placeholder="Category Name"
                      value={name}
                      onChange={(e) => setName(e.target.value)}
                      className="p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                      required
                    />
                    <button
                      type="submit"
                      className="bg-green-600 hover:bg-green-700 text-white py-3 rounded-lg font-semibold transition-all duration-300"
                    >
                      Save Category
                    </button>
                  </form>
                </Dialog.Content>
              </Dialog.Portal>
            </Dialog.Root>
          </div>

          {/* Search Bar */}
          <div className="flex justify-between items-center">
            <input
              type="text"
              placeholder="Search categories..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full max-w-sm p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          {/* Table */}
          <div className="overflow-x-auto bg-white rounded-lg shadow-md">
            <table className="min-w-full">
              <thead className="bg-gray-200">
                <tr>
                  <th className="py-3 px-6 text-left text-sm font-semibold">Name</th>
                  <th className="py-3 px-6 text-center text-sm font-semibold">Actions</th>
                </tr>
              </thead>
              <tbody>
                {currentCategories.length === 0 ? (
                  <tr>
                    <td colSpan={2} className="text-center py-10 text-gray-500">
                      No categories found.
                    </td>
                  </tr>
                ) : (
                  currentCategories.map((category) => (
                    <tr key={category.id} className="border-t hover:bg-gray-50 transition">
                      <td className="py-3 px-6">{category.name}</td>
                      <td className="py-3 px-6 flex justify-center gap-2">
                        <button
                          onClick={() => handleDelete(category.id)}
                          className="px-3 py-1 bg-red-600 hover:bg-red-700 text-white rounded-lg text-sm"
                        >
                          Delete
                        </button>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>

          {/* Pagination */}
          {totalPages > 1 && (
            <div className="flex justify-center items-center gap-2">
              {Array.from({ length: totalPages }, (_, index) => index + 1).map((page) => (
                <button
                  key={page}
                  onClick={() => setCurrentPage(page)}
                  className={`px-4 py-2 rounded-lg text-sm font-semibold ${
                    page === currentPage
                      ? "bg-blue-600 text-white"
                      : "bg-white border border-gray-300 text-gray-700 hover:bg-gray-100"
                  } transition`}
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
