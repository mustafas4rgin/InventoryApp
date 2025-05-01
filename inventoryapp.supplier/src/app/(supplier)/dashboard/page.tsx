'use client';
import Link from 'next/link';
import { useEffect, useState } from 'react';
import axiosInstance from '@/lib/axiosinstance';
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  Tooltip,
  ResponsiveContainer,
  CartesianGrid,
} from 'recharts';
import { Bell, Box, PackageCheck, CalendarPlus, Coins } from 'lucide-react';

// Tip tanımları
type Product = {
  id: number;
  name: string;
  stock: number;
  price: number;
  categoryId: number;
  category: {
    id: number;
    name: string;
  } | null;
  createdAt: string;
};

type User = {
  id: number;
  firstName: string;
  email: string;
  supplierId: number;
  createdAt?: string;
};

type Notification = {
  id: number;
  title: string;
  message: string;
  type: 'Info' | 'Warning' | 'Error';
  createdAt: string;
  status: 'Unread' | 'Read';
};

export default function DashboardPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [user, setUser] = useState<User | null>(null);
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [loading, setLoading] = useState(true);
  const [showAllNotifications, setShowAllNotifications] = useState(false);

  useEffect(() => {
    axiosInstance.get('/product/own')
      .then(res => setProducts(res.data))
      .catch(err => console.error('Ürünler alınamadı:', err));

    axiosInstance.get('/auth/me')
      .then(res => {
        setUser(res.data.user);
        return axiosInstance.get(`/Notification/user-notifications/${res.data.user.id}`);
      })
      .then(res => {
        const unread = (res.data.data ?? res.data).filter((n: Notification) => n.status === 'Unread');
        setNotifications(unread);
      })
      .catch(err => console.error('Kullanıcı veya bildirim bilgileri alınamadı:', err))
      .finally(() => setLoading(false));
  }, []);

  const markNotificationAsRead = async (id: number) => {
    try {
      await axiosInstance.put(`/notification/markasread/${id}`);
      setNotifications(prev => prev.filter(n => n.id !== id));
    } catch (error) {
      console.error('Bildirim okundu olarak işaretlenemedi:', error);
    }
  };

  const lowStockProducts = products.filter(p => p.stock < 10);
  const addedThisMonth = products.filter(p => {
    const created = new Date(p.createdAt);
    const now = new Date();
    return created.getMonth() === now.getMonth() && created.getFullYear() === now.getFullYear();
  });
  const totalStockValue = products.reduce((sum, p) => sum + p.price * p.stock, 0);

  function getGroupedByCategory(products: Product[]) {
    const map = new Map<string, number>();
    products.forEach(p => {
      const key = p.category?.name || 'Bilinmeyen';
      map.set(key, (map.get(key) || 0) + 1);
    });
    return Array.from(map.entries()).map(([category, count]) => ({ category, count }));
  }

  function getNotificationBorderColor(type: string) {
    switch (type) {
      case 'Warning':
        return 'border-yellow-500 dark:border-yellow-400';
      case 'Error':
        return 'border-red-500 dark:border-red-400';
      default:
        return 'border-blue-500 dark:border-blue-400';
    }
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64 text-gray-500 dark:text-gray-400">
        Yükleniyor...
      </div>
    );
  }

  return (
    <div className="min-h-screen p-6 bg-gray-50 dark:bg-[#0f172a]">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl md:text-4xl font-bold text-gray-900 dark:text-white">
          👋 Hoşgeldiniz, Tedarikçim!
        </h1>
        <button
          onClick={() => setShowAllNotifications(!showAllNotifications)}
          className="relative p-2 rounded-full bg-white dark:bg-gray-800 border dark:border-gray-600 hover:shadow"
        >
          <Bell className="w-6 h-6 text-gray-700 dark:text-white" />
          {notifications.length > 0 && (
            <span className="absolute top-0 right-0 w-4 h-4 text-[10px] rounded-full bg-red-500 text-white flex items-center justify-center">
              {notifications.length}
            </span>
          )}
        </button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-10">
        <Link href="/products" className="hover:scale-[1.02] transition-transform">
          <div className="bg-gradient-to-r from-blue-500 to-indigo-600 text-white rounded-xl shadow-lg p-6 flex flex-col gap-2">
            <Box className="w-6 h-6 opacity-80" />
            <p className="text-4xl font-bold break-words">{products.length}</p>
            <p className="text-sm">Toplam Ürün</p>
          </div>
        </Link>

        <Link href="/products?filter=lowstock" className="hover:scale-[1.02] transition-transform">
          <div className="bg-gradient-to-r from-yellow-500 to-yellow-600 text-white rounded-xl shadow-lg p-6 flex flex-col gap-2">
            <PackageCheck className="w-6 h-6 opacity-80" />
            <p className="text-4xl font-bold break-words">{lowStockProducts.length}</p>
            <p className="text-sm">Düşük Stoklu Ürün</p>
          </div>
        </Link>

        <div className="bg-gradient-to-r from-green-500 to-green-600 text-white rounded-xl shadow-lg p-6 flex flex-col gap-2">
          <CalendarPlus className="w-6 h-6 opacity-80" />
          <p className="text-4xl font-bold break-words">{addedThisMonth.length}</p>
          <p className="text-sm">Bu Ay Eklenen</p>
        </div>

        <div className="bg-gradient-to-r from-pink-500 to-rose-500 text-white rounded-xl shadow-lg p-6 flex flex-col gap-2">
          <Coins className="w-6 h-6 opacity-80" />
          <p className="text-3xl font-bold truncate">₺{totalStockValue.toLocaleString('tr-TR', { minimumFractionDigits: 2 })}</p>
          <p className="text-sm">Toplam Stok Değeri</p>
        </div>
      </div>

      {showAllNotifications && (
        <div className="mb-6 p-4 rounded-xl bg-white dark:bg-gray-800 border dark:border-gray-700 shadow">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-lg font-semibold text-gray-900 dark:text-white">🔔 Bildirimler</h2>
            <button onClick={() => setShowAllNotifications(false)} className="text-sm text-gray-600 dark:text-gray-400 hover:underline">
              Kapat ✖
            </button>
          </div>
          {notifications.length === 0 ? (
            <p className="text-sm text-gray-500 dark:text-gray-400">Hiç bildirim yok.</p>
          ) : (
            <ul className="space-y-3 max-h-72 overflow-y-auto pr-2">
              {notifications.map(n => (
                <li
                  key={n.id}
                  className={`relative border-l-4 pl-3 pr-6 bg-gray-50 dark:bg-gray-700 rounded-md shadow-sm dark:shadow-none ${getNotificationBorderColor(n.type)}`}
                >
                  <button
                    onClick={() => markNotificationAsRead(n.id)}
                    className="absolute top-2 right-2 text-gray-500 hover:text-red-500"
                  >✕</button>
                  <p className="text-sm font-semibold text-gray-800 dark:text-white">{n.title}</p>
                  <p className="text-xs text-gray-600 dark:text-gray-300">{n.message}</p>
                  <p className="text-[10px] text-gray-400 dark:text-gray-500 mt-1">
                    {new Date(n.createdAt).toLocaleString('tr-TR')}
                  </p>
                </li>
              ))}
            </ul>
          )}
        </div>
      )}

      {user && (
        <div className="mb-8 p-6 rounded-xl bg-gradient-to-r from-indigo-500 via-purple-500 to-pink-500 text-white shadow-lg">
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
            <div>
              <h2 className="text-2xl font-bold">{user.firstName}</h2>
              <p className="text-sm opacity-90">{user.email}</p>
            </div>
            <div className="bg-white/20 px-4 py-2 rounded-full text-sm font-medium">
              Tedarikçi {user.firstName}
            </div>
          </div>
        </div>
      )}

      <div className="mb-12">
        <h2 className="text-2xl font-semibold text-gray-800 dark:text-white mb-4">
          📉 Stokta Azalan Ürünler
        </h2>
        {lowStockProducts.length === 0 ? (
          <p className="text-green-600 dark:text-green-400">
            Tüm ürünlerin stoğu yeterli görünüyor.
          </p>
        ) : (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
            {lowStockProducts.map(product => (
              <div key={product.id} className="rounded-xl bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 shadow hover:shadow-md transition p-5">
                <h3 className="text-lg font-semibold text-gray-900 dark:text-white mb-2">
                  {product.name}
                </h3>
                <p className="text-sm text-red-600 dark:text-red-400">
                  Stok: {product.stock}
                </p>
              </div>
            ))}
          </div>
        )}
      </div>

      <div className="mt-12">
        <h2 className="text-2xl font-semibold text-gray-800 dark:text-white mb-4">
          🆕 Son Eklenen Ürünler
        </h2>
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          {[...products]
            .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
            .slice(0, 6)
            .map(product => (
              <div key={product.id} className="bg-white dark:bg-gray-800 border dark:border-gray-700 rounded-xl p-5 shadow hover:shadow-md transition">
                <h3 className="text-lg font-semibold text-gray-900 dark:text-white mb-1">
                  {product.name}
                </h3>
                <p className="text-sm text-gray-600 dark:text-gray-300">
                  Kategori: {product.category?.name ?? 'Bilinmiyor'}
                </p>
                <p className="text-sm text-gray-600 dark:text-gray-300">
                  Fiyat: ₺{product.price}
                </p>
                <p className="text-sm text-gray-500 mt-1">
                  Eklendi: {new Date(product.createdAt).toLocaleDateString('tr-TR')}
                </p>
              </div>
            ))}
        </div>
      </div>
    </div>
  );
}
