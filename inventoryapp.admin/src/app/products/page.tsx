"use client";

import { useEffect, useState } from "react";
import axiosInstance from "@/lib/axiosInstance";
import AuthProvider from "@/components/AuthProvider";
import Sidebar from "@/components/Sidebar";
import AddProductModal from "@/components/AddProductModal";
import EditProductModal from "@/components/EditProductModal";

interface Product {
    id: number;
    name: string;
    price: number;
    stock: number;
    categoryId: number;
    supplierId: number;
  }
  

export default function ProductsPage() {
    const [products, setProducts] = useState<Product[]>([]);
    const [searchTerm, setSearchTerm] = useState("");
    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage] = useState(5);

    const fetchProducts = async () => {
        const res = await axiosInstance.get("/Product/GetAll");
        setProducts(res.data);
    };

    useEffect(() => {
        fetchProducts();
    }, []);

    const handleDelete = async (id: number) => {
        if (confirm("Are you sure you want to delete?")) {
            await axiosInstance.delete(`/Product/${id}`);
            fetchProducts();
        }
    };

    const filteredProducts = products.filter((product) =>
        product.name.toLowerCase().includes(searchTerm.toLowerCase())
    );

    const totalPages = Math.ceil(filteredProducts.length / itemsPerPage);
    const indexOfLastItem = currentPage * itemsPerPage;
    const indexOfFirstItem = indexOfLastItem - itemsPerPage;
    const currentProducts = filteredProducts.slice(indexOfFirstItem, indexOfLastItem);

    const handlePageChange = (page: number) => {
        setCurrentPage(page);
    };

    return (
        <AuthProvider>
            <div className="flex min-h-screen bg-gray-100">
                <Sidebar />
                <div className="flex-1 p-8 space-y-8">
                    <div className="flex justify-between items-center">
                        <h1 className="text-3xl font-bold text-gray-800">Products</h1>
                        <AddProductModal onSuccess={fetchProducts} />
                    </div>

                    <div className="flex justify-between items-center">
                        <input
                            type="text"
                            placeholder="Search products..."
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                            className="w-full max-w-sm p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                        />
                    </div>

                    <div className="overflow-x-auto bg-white rounded-lg shadow-md">
                        <table className="min-w-full">
                            <thead className="bg-gray-200">
                                <tr>
                                    <th className="py-3 px-6 text-left text-sm font-semibold">Name</th>
                                    <th className="py-3 px-6 text-left text-sm font-semibold">Price</th>
                                    <th className="py-3 px-6 text-left text-sm font-semibold">Stock</th>
                                    <th className="py-3 px-6 text-center text-sm font-semibold">Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                {currentProducts.length === 0 ? (
                                    <tr>
                                        <td colSpan={4} className="text-center py-10 text-gray-500">
                                            No products found.
                                        </td>
                                    </tr>
                                ) : (
                                    currentProducts.map((product) => (
                                        <tr key={product.id} className="border-t hover:bg-gray-50 transition">
                                            <td className="py-3 px-6">{product.name}</td>
                                            <td className="py-3 px-6">₺{product.price.toFixed(2)}</td>
                                            <td className="py-3 px-6">{product.stock}</td>
                                            <td className="py-3 px-6 flex justify-center gap-2">
                                                <td className="py-3 px-6 flex justify-center gap-2">
                                                    <EditProductModal product={product} onSuccess={fetchProducts} />
                                                    <button
                                                        onClick={() => handleDelete(product.id)}
                                                        className="px-3 py-1 bg-red-600 hover:bg-red-700 text-white rounded-lg text-sm"
                                                    >
                                                        Delete
                                                    </button>
                                                </td>
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
                                    onClick={() => handlePageChange(page)}
                                    className={`px-4 py-2 rounded-lg text-sm font-semibold ${page === currentPage
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
