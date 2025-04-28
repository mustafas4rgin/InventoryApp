"use client";

import { useEffect, useState } from "react";
import axiosInstance from "@/lib/axiosInstance";
import AuthProvider from "@/components/AuthProvider";
import Sidebar from "@/components/Sidebar";
import { toast } from "react-hot-toast";
import { format, isToday, isYesterday, differenceInDays } from "date-fns";
import { Loader2, CheckCircle, Trash2, Bell } from "lucide-react";

interface Notification {
  id: number;
  title: string;
  message: string;
  createdAt: string;
  status: "Unread" | "Read";
}

export default function NotificationsPage() {
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [activeTab, setActiveTab] = useState<"Unread" | "Read">("Unread");
  const [loadingId, setLoadingId] = useState<number | null>(null);

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

  const handleMarkAsRead = async (id: number) => {
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
      toast.error("Failed to mark notification as read.");
      setLoadingId(null);
    }
  };

  const handleDeleteNotification = async (id: number) => {
    const confirmDelete = window.confirm("Are you sure you want to delete this notification?");
    if (!confirmDelete) return;

    try {
      await axiosInstance.delete(`/Notification/${id}`);
      toast.success("Notification deleted!");
      setNotifications((prev) => prev.filter((n) => n.id !== id));
    } catch (error) {
      console.error(error);
      toast.error("Failed to delete notification.");
    }
  };

  const handleMarkAllAsRead = async () => {
    try {
      const unreadNotifications = notifications.filter((n) => n.status === "Unread");
      if (unreadNotifications.length === 0) return;

      await Promise.all(
        unreadNotifications.map((notification) =>
          axiosInstance.put(`/Notification/${notification.id}`)
        )
      );

      setNotifications((prev) =>
        prev.map((n) => ({ ...n, status: "Read" }))
      );

      toast.success("All notifications marked as read!");
    } catch (error) {
      console.error(error);
      toast.error("Failed to mark all as read.");
    }
  };

  const filteredNotifications = notifications.filter(
    (n) => n.status === activeTab
  );

  const groupedNotifications = {
    Today: filteredNotifications.filter((n) => isToday(new Date(n.createdAt))),
    Yesterday: filteredNotifications.filter((n) => isYesterday(new Date(n.createdAt))),
    Last7Days: filteredNotifications.filter(
      (n) => !isToday(new Date(n.createdAt)) && !isYesterday(new Date(n.createdAt)) && differenceInDays(new Date(), new Date(n.createdAt)) <= 7
    ),
    Older: filteredNotifications.filter(
      (n) => differenceInDays(new Date(), new Date(n.createdAt)) > 7 && differenceInDays(new Date(), new Date(n.createdAt)) <= 30
    ),
  };

  return (
    <AuthProvider>
      <div className="flex min-h-screen bg-gray-100">
        <Sidebar />
        <div className="flex-1 p-8 space-y-6">
          <h1 className="text-3xl font-bold text-gray-800">Notifications</h1>

          <div className="flex gap-4 mb-4">
            <button
              onClick={() => setActiveTab("Unread")}
              className={`px-4 py-2 rounded-lg text-sm font-medium transition ${
                activeTab === "Unread" ? "bg-blue-600 text-white" : "bg-gray-200 text-gray-700"
              }`}
            >
              Unread
            </button>
            <button
              onClick={() => setActiveTab("Read")}
              className={`px-4 py-2 rounded-lg text-sm font-medium transition ${
                activeTab === "Read" ? "bg-blue-600 text-white" : "bg-gray-200 text-gray-700"
              }`}
            >
              Read
            </button>
            {activeTab === "Unread" && filteredNotifications.length > 0 && (
              <button
                onClick={handleMarkAllAsRead}
                className="ml-auto px-4 py-2 rounded-lg bg-green-600 text-white text-sm font-semibold hover:bg-green-700 transition"
              >
                Mark All as Read
              </button>
            )}
          </div>

          <div className="overflow-x-auto bg-white rounded-lg shadow-md">
            {filteredNotifications.length === 0 ? (
              <div className="flex flex-col items-center justify-center py-20 text-gray-500 animate-pulse">
                <Bell size={60} className="mb-4" />
                <p className="text-lg">No notifications yet 🎉</p>
              </div>
            ) : (
              <div className="p-4">
                {Object.entries(groupedNotifications).map(([group, notifs]) => (
                  notifs.length > 0 && (
                    <div key={group} className="mb-8">
                      <h2 className="text-xl font-semibold text-gray-700 mb-4">{group}</h2>
                      <div className="space-y-4">
                        {notifs.map((notification) => (
                          <div key={notification.id} className="p-4 border rounded-lg flex justify-between items-center hover:bg-gray-50 transition">
                            <div>
                              <h3 className="font-medium text-gray-800">{notification.title}</h3>
                              <p className="text-gray-600 text-sm">{notification.message}</p>
                              <p className="text-gray-400 text-xs mt-1">{format(new Date(notification.createdAt), "dd.MM.yyyy HH:mm")}</p>
                            </div>
                            <div className="flex gap-2">
                              {notification.status === "Unread" && (
                                <button
                                  onClick={() => handleMarkAsRead(notification.id)}
                                  disabled={loadingId === notification.id}
                                  title="Mark as Read"
                                  className="p-2 bg-green-600 hover:bg-green-700 text-white rounded-lg text-sm transition disabled:opacity-50 disabled:cursor-not-allowed"
                                >
                                  {loadingId === notification.id ? (
                                    <Loader2 className="animate-spin" size={18} />
                                  ) : (
                                    <CheckCircle size={18} />
                                  )}
                                </button>
                              )}
                              <button
                                onClick={() => handleDeleteNotification(notification.id)}
                                title="Delete Notification"
                                className="p-2 text-red-600 hover:text-red-800 transition"
                              >
                                <Trash2 size={18} />
                              </button>
                            </div>
                          </div>
                        ))}
                      </div>
                    </div>
                  )
                ))}
              </div>
            )}
          </div>
        </div>
      </div>
    </AuthProvider>
  );
}
