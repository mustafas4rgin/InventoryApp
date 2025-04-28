"use client";

import { useEffect, useState } from "react";
import axiosInstance from "@/lib/axiosInstance";
import { useRouter } from "next/navigation";
import debounce from "lodash.debounce";
import Link from "next/link";

interface Supplier {
  id: number;
  name: string;
}

export default function RegisterPage() {
  const router = useRouter();

  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [passwordMatch, setPasswordMatch] = useState("");
  const [supplierSearch, setSupplierSearch] = useState("");
  const [suppliers, setSuppliers] = useState<Supplier[]>([]);
  const [selectedSupplierId, setSelectedSupplierId] = useState<number | null>(null);
  const [loading, setLoading] = useState(false);

  // Supplier arama
  const fetchSuppliers = async (search: string) => {
    if (!search) {
      setSuppliers([]);
      return;
    }
    try {
      const res = await axiosInstance.get(`/Supplier/GetAll?include=users&search=${search}`);
      setSuppliers(res.data);
    } catch (error) {
      console.error(error);
    }
  };

  const debouncedFetchSuppliers = debounce(fetchSuppliers, 300);

  useEffect(() => {
    debouncedFetchSuppliers(supplierSearch);
    return () => {
      debouncedFetchSuppliers.cancel();
    };
  }, [supplierSearch]);

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();

    if (password !== passwordMatch) {
      alert("Passwords do not match!");
      return;
    }

    if (!selectedSupplierId) {
      alert("Please select a supplier!");
      return;
    }

    setLoading(true);
    try {
      await axiosInstance.post("/auth/register", {
        firstName,
        lastName,
        email,
        password,
        passwordMatch,
        roleId: 3, // Burada sabit 3 gönderiyoruz
        supplierId: selectedSupplierId,
      });
      alert("Registration successful! Please login.");
      router.push("/login");
    } catch (error) {
      console.error(error);
      alert("Registration failed!");
    }
    setLoading(false);
  };

  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-100">
      <form onSubmit={handleRegister} className="bg-white p-8 rounded shadow-md w-full max-w-md space-y-6">
        <h2 className="text-2xl font-bold text-center">Register</h2>

        <input
          type="text"
          placeholder="First Name"
          value={firstName}
          onChange={(e) => setFirstName(e.target.value)}
          className="w-full p-2 border border-gray-300 rounded"
          required
        />

        <input
          type="text"
          placeholder="Last Name"
          value={lastName}
          onChange={(e) => setLastName(e.target.value)}
          className="w-full p-2 border border-gray-300 rounded"
          required
        />

        <input
          type="email"
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          className="w-full p-2 border border-gray-300 rounded"
          required
        />

        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          className="w-full p-2 border border-gray-300 rounded"
          required
        />

        <input
          type="password"
          placeholder="Confirm Password"
          value={passwordMatch}
          onChange={(e) => setPasswordMatch(e.target.value)}
          className="w-full p-2 border border-gray-300 rounded"
          required
        />

        <input
          type="text"
          placeholder="Search Supplier..."
          value={supplierSearch}
          onChange={(e) => setSupplierSearch(e.target.value)}
          className="w-full p-2 border border-gray-300 rounded"
        />

        {suppliers.length > 0 && (
          <ul className="border border-gray-300 rounded mt-2 max-h-40 overflow-y-auto">
            {suppliers.map((supplier) => (
              <li
                key={supplier.id}
                onClick={() => {
                  setSupplierSearch(supplier.name);
                  setSelectedSupplierId(supplier.id);
                  setSuppliers([]);
                }}
                className="p-2 hover:bg-gray-100 cursor-pointer"
              >
                {supplier.name}
              </li>
            ))}
          </ul>
        )}

        <button
          type="submit"
          disabled={loading}
          className="w-full bg-blue-600 hover:bg-blue-700 text-white py-2 rounded transition disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {loading ? "Registering..." : "Register"}
        </button>

        {/* Already have an account */}
        <p className="text-center text-sm text-gray-600 mt-4">
          Already have an account?{" "}
          <Link href="/login" className="text-blue-600 hover:underline">
            Login
          </Link>
        </p>
      </form>
    </div>
  );
}
