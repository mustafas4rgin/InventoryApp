"use client";

import { Bell, LayoutDashboard, Users, Box, List, Truck, LogOut, XCircle } from "lucide-react";
import { useState, useEffect } from "react";
import Link from "next/link";
import axiosInstance from "@/lib/axiosInstance";
import { toast } from "react-hot-toast";
import { destroyCookie } from "nookies";
import { useRouter } from "next/navigation";
import { format, isToday, isYesterday, differenceInDays } from "date-fns";


interface Notification {
  id: number;
  title: string;
  message: string;
  createdAt: string;
  status: "Unread" | "Read";
}

export default function Sidebar() {
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [isOpen, setIsOpen] = useState(false);
  const [loadingId, setLoadingId] = useState<number | null>(null);
  const router = useRouter();

  const fetchNotifications = async () => {
    try {
      const res = await axiosInstance.get("/Notification/GetAll");
  

      if (res.status === 200) {
        const filtered = res.data.filter((n: Notification) => differenceInDays(new Date(), new Date(n.createdAt)) <= 30);
        setNotifications(filtered);
      }
    } catch (error: any) {

      if (error.response?.status !== 404) { 
        toast.error("Failed to fetch notifications.");
      }
      setNotifications([]);
    }
  };
  

  useEffect(() => {
    fetchNotifications();
  }, []);

  const handleNotificationClick = async (id: number) => {
    const notification = notifications.find((n) => n.id === id);
    if (!notification || notification.status === "Read") {
      return;
    }

    try {
      setLoadingId(id);
      await axiosInstance.put(`/Notification/${id}`);
      toast.success("Notification marked as read!");
      setNotifications((prev) =>
        prev.map((n) => (n.id === id ? { ...n, status: "Read" } : n))
      );
      setLoadingId(null);
    } catch (error) {
      console.error(error);
      toast.error("Failed to mark as read.");
      setLoadingId(null);
    }
  };

  const handleDeleteNotification = async (id: number) => {

    try {
      await axiosInstance.delete(`/Notification/Delete/${id}?hard=true`);
      toast.success("Notification deleted!");
      setNotifications((prev) => prev.filter((n) => n.id !== id));
    } catch (error) {
      console.error(error);
      toast.error("Failed to delete notification.");
    }
  };

  const handleClearAllNotifications = async () => {
    const confirmClear = window.confirm("Are you sure you want to delete ALL notifications?");
    if (!confirmClear) return;

    try {
      await Promise.all(
        notifications.map((notification) =>
          axiosInstance.delete(`/Notification/Delete/${notification.id}?hard=true`)
        )
      );
      setNotifications([]);
      toast.success("All notifications cleared!");
    } catch (error) {
      console.error(error);
      toast.error("Failed to clear notifications.");
    }
  };

  const handleLogout = () => {
    destroyCookie(null, "accessToken");
    destroyCookie(null, "userRole");
    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");
    localStorage.removeItem("userRole");
    router.push("/login");
  };

  return (
    <div className="w-64 bg-white border-r border-gray-200 min-h-screen p-6 flex flex-col justify-between">
      {/* Üst Kısım */}
      <div className="space-y-6">
        {/* Bildirim Zili */}
        <div className="flex justify-end">
          <button
            onClick={() => setIsOpen(!isOpen)}
            className="relative text-gray-700 hover:text-blue-600 transition"
          >
            <Bell size={24} />
            {notifications.filter((n) => n.status === "Unread").length > 0 && (
              <span className="absolute -top-2 -right-2 inline-flex items-center justify-center w-5 h-5 text-xs font-bold text-white bg-red-500 rounded-full animate-bounce">
                {notifications.filter((n) => n.status === "Unread").length}
              </span>
            )}
          </button>
        </div>

        {/* Bildirim Açılır Menü */}
        {isOpen && (
          <div className="absolute left-64 top-10 w-80 bg-white rounded-lg shadow-lg overflow-hidden animate-fadeIn z-50">
            {notifications.length === 0 ? (
              <div className="p-4 text-center text-gray-400">No notifications 🎉</div>
            ) : (
              <>
                <button
                  onClick={handleClearAllNotifications}
                  className="w-full bg-red-500 text-white py-2 text-sm font-semibold hover:bg-red-600 transition"
                >
                  Clear All Notifications
                </button>
                {notifications.map((notification) => (
                  <div
                    key={notification.id}
                    className="p-4 border-b hover:bg-gray-100 transition flex justify-between items-start gap-2"
                  >
                    <div
                      onClick={() => handleNotificationClick(notification.id)}
                      className="flex-1 cursor-pointer"
                    >
                      <h3 className="font-semibold text-gray-800 flex items-center">
                        {notification.title || "Notification"}
                        {notification.status === "Unread" && (
                          <span className="ml-2 inline-block w-2 h-2 bg-red-500 rounded-full"></span>
                        )}
                      </h3>
                      <p className="text-sm text-gray-600">{notification.message || "No details."}</p>
                    </div>
                    <button
                      onClick={() => handleDeleteNotification(notification.id)}
                      className="text-red-600 hover:text-red-800 transition"
                    >
                      <XCircle size={20} />
                    </button>
                  </div>
                ))}
              </>
            )}
          </div>
        )}

        {/* Normal Menü */}
        <div className="flex flex-col gap-4 mt-6">
          <Link href="/dashboard" className="flex items-center gap-3 text-gray-800 hover:text-blue-600 font-semibold transition">
            <LayoutDashboard size={20} />
            Dashboard
          </Link>
          <Link href="/users" className="flex items-center gap-3 text-gray-800 hover:text-blue-600 font-semibold transition">
            <Users size={20} />
            Users
          </Link>
          <Link href="/products" className="flex items-center gap-3 text-gray-800 hover:text-blue-600 font-semibold transition">
            <Box size={20} />
            Products
          </Link>
          <Link href="/categories" className="flex items-center gap-3 text-gray-800 hover:text-blue-600 font-semibold transition">
            <List size={20} />
            Categories
          </Link>
          <Link href="/suppliers" className="flex items-center gap-3 text-gray-800 hover:text-blue-600 font-semibold transition">
            <Truck size={20} />
            Suppliers
          </Link>
          <Link href="/notifications" className="flex items-center gap-3 text-gray-800 hover:text-blue-600 font-semibold transition">
            <Bell size={20} />
            Notifications
          </Link>
        </div>
      </div>

      {/* Alt Kısım */}
      <div className="flex flex-col gap-4">
        <button
          onClick={handleLogout}
          className="w-full flex items-center justify-center gap-2 py-2 px-4 bg-red-500 hover:bg-red-600 text-white rounded-lg font-semibold transition-all duration-300 hover:scale-105"
        >
          <LogOut size={18} />
          Logout
        </button>
        <div className="text-xs text-gray-400 text-center">&copy; 2025 InventoryApp</div>
      </div>
    </div>
  );
}
