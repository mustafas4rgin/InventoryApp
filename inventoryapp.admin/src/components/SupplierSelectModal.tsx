"use client";

import * as Dialog from "@radix-ui/react-dialog";
import { useState, useEffect } from "react";
import { X } from "lucide-react";
import axiosInstance from "@/lib/axiosInstance";

interface Supplier {
  id: number;
  name: string;
}

interface SupplierSelectModalProps {
  onSelect: (supplierId: number) => void;
}

export default function SupplierSelectModal({ onSelect }: SupplierSelectModalProps) {
  const [open, setOpen] = useState(false);
  const [suppliers, setSuppliers] = useState<Supplier[]>([]);
  const [searchTerm, setSearchTerm] = useState("");

  useEffect(() => {
    if (open) {
      fetchSuppliers();
    }
  }, [open]);

  const fetchSuppliers = async () => {
    try {
      const res = await axiosInstance.get("/suppliers");
      setSuppliers(res.data);
    } catch (error) {
      console.error("Failed to fetch suppliers", error);
    }
  };

  const filteredSuppliers = suppliers
    .filter((supplier) => supplier.name.toLowerCase().includes(searchTerm.toLowerCase()))
    .slice(0, 3);

  const handleSelectSupplier = (id: number) => {
    onSelect(id);
    setOpen(false);
  };

  return (
    <Dialog.Root open={open} onOpenChange={setOpen}>
      <Dialog.Trigger asChild>
        <button
          type="button"
          className="bg-indigo-600 hover:bg-indigo-700 text-white px-4 py-2 rounded-lg font-semibold shadow-md transition-all duration-300"
        >
          Select Supplier
        </button>
      </Dialog.Trigger>

      <Dialog.Portal>
        <Dialog.Overlay className="fixed inset-0 bg-black/40 backdrop-blur-sm transition-all" />
        <Dialog.Content className="fixed top-1/2 left-1/2 w-full max-w-md -translate-x-1/2 -translate-y-1/2 bg-white p-6 rounded-xl shadow-lg space-y-6 animate-fadeIn">
          <div className="flex justify-between items-center">
            <Dialog.Title className="text-2xl font-bold text-gray-800">Select Supplier</Dialog.Title>
            <Dialog.Close asChild>
              <button className="text-gray-400 hover:text-gray-600 transition">
                <X size={24} />
              </button>
            </Dialog.Close>
          </div>

          {/* Search input */}
          <input
            type="text"
            placeholder="Search suppliers..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500"
          />

          {/* Suppliers List */}
          <div className="flex flex-col gap-4">
            {filteredSuppliers.length === 0 ? (
              <p className="text-gray-500 text-center">No suppliers found.</p>
            ) : (
              filteredSuppliers.map((supplier) => (
                <button
                  key={supplier.id}
                  onClick={() => handleSelectSupplier(supplier.id)}
                  className="w-full p-3 bg-gray-100 hover:bg-gray-200 rounded-lg text-left font-medium text-gray-800 transition"
                >
                  {supplier.name}
                </button>
              ))
            )}
          </div>
        </Dialog.Content>
      </Dialog.Portal>
    </Dialog.Root>
  );
}
