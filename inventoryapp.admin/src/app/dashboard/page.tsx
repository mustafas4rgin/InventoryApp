"use client";

import { useEffect, useState } from "react";
import axiosInstance from "@/lib/axiosInstance";
import AuthProvider from "@/components/AuthProvider";
import Sidebar from "@/components/Sidebar";

export default function DashboardPage() {
  const [productCount, setProductCount] = useState(0);
  const [categoryCount, setCategoryCount] = useState(0);
  const [supplierCount, setSupplierCount] = useState(0);

  const fetchData = async () => {
    try {
      const productsRes = await axiosInstance.get("/Product/GetAll");
      setProductCount(productsRes.data.length);

      const categoriesRes = await axiosInstance.get("/Category/GetAll");
      setCategoryCount(categoriesRes.data.length);

      const suppliersRes = await axiosInstance.get("/Supplier/GetAll");
      setSupplierCount(suppliersRes.data.length);
    } catch (error) {
      console.error(error);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  return (
    <AuthProvider>
        <div className="flex min-h-screen bg-gray-100">
        <Sidebar />
        <div className="flex-1 p-8">
          <h1 className="text-3xl font-bold mb-8 text-gray-800">Dashboard</h1>

          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-6 animate-fadeIn">
            <div className="bg-white p-6 rounded-xl shadow-md hover:shadow-lg transition-all duration-300 flex flex-col items-center">
              <div className="text-4xl font-bold text-blue-600">{productCount}</div>
              <div className="text-gray-600 mt-2 text-lg font-semibold">Products</div>
            </div>

            <div className="bg-white p-6 rounded-xl shadow-md hover:shadow-lg transition-all duration-300 flex flex-col items-center">
              <div className="text-4xl font-bold text-green-600">{categoryCount}</div>
              <div className="text-gray-600 mt-2 text-lg font-semibold">Categories</div>
            </div>

            <div className="bg-white p-6 rounded-xl shadow-md hover:shadow-lg transition-all duration-300 flex flex-col items-center">
              <div className="text-4xl font-bold text-indigo-600">{supplierCount}</div>
              <div className="text-gray-600 mt-2 text-lg font-semibold">Suppliers</div>
            </div>
          </div>
        </div>
      </div>
    </AuthProvider>
      
  );
}
