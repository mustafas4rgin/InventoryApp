import { AuthProvider } from "@/context/AuthContext";
import "./globals.css";
import { Toaster } from "react-hot-toast";

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en" suppressHydrationWarning>
      <body className="min-h-screen bg-gray-50 dark:bg-[#0f172a] text-gray-900 dark:text-gray-100 font-sans">
        <AuthProvider>
        <Toaster position="top-right" />
        {children}
        </AuthProvider>
        
      </body>
    </html>
  );
}
