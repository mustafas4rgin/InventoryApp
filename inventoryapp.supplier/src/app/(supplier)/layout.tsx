import Sidebar from "@/components/supplierLayout";

export default function SupplierLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="flex">
      <Sidebar />
      <main className="flex-1 min-h-screen bg-gray-50 dark:bg-[#0f172a] p-6">
        {children}
      </main>
    </div>
  );
}
