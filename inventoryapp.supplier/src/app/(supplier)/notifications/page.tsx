'use client';

import { useEffect, useState } from 'react';
import axiosInstance from '@/lib/axiosinstance';
import toast from 'react-hot-toast';
import {
  LineChart,
  Line,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  Tooltip,
  ResponsiveContainer,
  CartesianGrid,
  PieChart,
  Pie,
  Cell,
  Legend,
} from 'recharts';

export default function NotificationsPage() {
  const [notifications, setNotifications] = useState<any[]>([]);
  const [userId, setUserId] = useState<number | null>(null);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState<'all' | 'read' | 'unread'>('all');

  useEffect(() => {
    axiosInstance.get('/auth/me')
      .then(res => {
        const id = res.data.user.id;
        setUserId(id);
        return axiosInstance.get(`/notification/user-notifications/${id}`);
      })
      .then(res => {
        setNotifications(res.data.data || res.data);
      })
      .catch(err => {
        toast.error('Bildirimler alınamadı');
      })
      .finally(() => setLoading(false));
  }, []);

  const handleMarkAsRead = async (notificationId: number) => {
    try {
      await axiosInstance.put(`/notification/${notificationId}`, { status: 'Read' });
      setNotifications(prev => prev.map(n =>
        n.id === notificationId ? { ...n, status: 'Read' } : n
      ));
      toast.success('Bildirim okundu olarak işaretlendi');
    } catch (err) {
      toast.error('İşaretleme başarısız');
    }
  };

  const handleMarkAllAsRead = async () => {
    try {
      const unreadIds = notifications.filter(n => n.status === 'Unread').map(n => n.id);
      await Promise.all(unreadIds.map(id => axiosInstance.put(`/notification/${id}`, { status: 'Read' })));
      setNotifications(prev => prev.map(n => ({ ...n, status: 'Read' })));
      toast.success('Tüm bildirimler okundu olarak işaretlendi');
    } catch {
      toast.error('Tümünü işaretleme başarısız');
    }
  };

  const handleDelete = async (notificationId: number) => {
    try {
      await axiosInstance.delete(`/notification/delete/${notificationId}`);
      setNotifications(prev => prev.filter(n => n.id !== notificationId));
      toast.success('Bildirim silindi');
    } catch {
      toast.error('Silme başarısız');
    }
  };

  const filteredNotifications =
    filter === 'all' ? notifications :
    filter === 'read' ? notifications.filter(n => n.status === 'Read') :
    notifications.filter(n => n.status === 'Unread');

  const colors = {
    Info: '#3b82f6',
    Warning: '#facc15',
    Error: '#ef4444',
    Read: '#10b981',
    Unread: '#3b82f6',
  };

  return (
    <div className="min-h-screen bg-gray-50 dark:bg-[#0f172a] p-6 flex flex-col items-center justify-center gap-12">
      <div className="w-full max-w-2xl bg-white dark:bg-gray-900 rounded-xl shadow p-6">
        <div className="flex items-center justify-between mb-4">
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white">🔔 Bildirimler</h1>
          <div className="text-sm text-gray-700 dark:text-gray-300">
            Okunmamış: {notifications.filter(n => n.status === 'Unread').length}
          </div>
        </div>

        <div className="flex gap-2 mb-4">
          <button onClick={() => setFilter('all')} className={`px-3 py-1 rounded ${filter === 'all' ? 'bg-blue-600 text-white' : 'bg-gray-200 dark:bg-gray-800 text-gray-800 dark:text-gray-200'}`}>Tümü</button>
          <button onClick={() => setFilter('unread')} className={`px-3 py-1 rounded ${filter === 'unread' ? 'bg-blue-600 text-white' : 'bg-gray-200 dark:bg-gray-800 text-gray-800 dark:text-gray-200'}`}>Okunmamış</button>
          <button onClick={() => setFilter('read')} className={`px-3 py-1 rounded ${filter === 'read' ? 'bg-blue-600 text-white' : 'bg-gray-200 dark:bg-gray-800 text-gray-800 dark:text-gray-200'}`}>Okunan</button>
          <button onClick={handleMarkAllAsRead} className="ml-auto px-3 py-1 rounded bg-green-600 text-white hover:bg-green-700">Tümünü Gördüm</button>
        </div>

        {loading ? (
          <p className="text-gray-500 dark:text-gray-400">Yükleniyor...</p>
        ) : filteredNotifications.length === 0 ? (
          <p className="text-gray-600 dark:text-gray-300">Hiç bildirim yok.</p>
        ) : (
          <ul className="space-y-4">
            {filteredNotifications.map(notification => (
              <li
                key={notification.id}
                className={`relative p-4 rounded-lg border-l-4 shadow-sm flex justify-between items-start gap-4 ${
                  notification.status === 'Unread' ? 'bg-blue-50 dark:bg-blue-900' : 'bg-green-50 dark:bg-green-900'
                } ${
                  notification.type === 'Error' ? 'border-red-500' :
                  notification.type === 'Warning' ? 'border-yellow-500' :
                  'border-blue-500'
                }`}
              >
                <div className="flex-1">
                  <p className="font-semibold text-gray-800 dark:text-white flex items-center gap-2">
                    {notification.title}
                    {notification.status === 'Unread' && <span className="w-2 h-2 rounded-full bg-red-500 animate-pulse" />}
                  </p>
                  <p className="text-sm text-gray-600 dark:text-gray-300">{notification.message}</p>
                  <p className="text-xs text-gray-400 dark:text-gray-500 mt-1">
                    {new Date(notification.createdAt).toLocaleString('tr-TR')}
                  </p>
                </div>
                <div className="flex flex-col items-end gap-2">
                  {notification.status === 'Unread' && (
                    <button
                      onClick={() => handleMarkAsRead(notification.id)}
                      className="text-sm text-blue-600 hover:underline dark:text-blue-400"
                    >
                      Gördüm
                    </button>
                  )}
                  <button
                    onClick={() => handleDelete(notification.id)}
                    className="text-sm text-red-500 hover:underline dark:text-red-400"
                  >
                    Sil
                  </button>
                </div>
              </li>
            ))}
          </ul>
        )}
      </div>

      <div className="w-full max-w-2xl bg-white dark:bg-gray-900 rounded-xl shadow p-6">
        <h2 className="text-xl font-bold text-gray-900 dark:text-white mb-4">📈 Günlük Bildirim Dağılımı</h2>
        <ResponsiveContainer width="100%" height={300}>
          <LineChart data={getChartData(notifications)}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis dataKey="date" />
            <YAxis allowDecimals={false} />
            <Tooltip />
            <Line type="monotone" dataKey="count" stroke="#4f46e5" strokeWidth={2} />
          </LineChart>
        </ResponsiveContainer>
      </div>

      <div className="w-full max-w-2xl bg-white dark:bg-gray-900 rounded-xl shadow p-6">
        <h2 className="text-xl font-bold text-gray-900 dark:text-white mb-4">📊 Okunma Durumuna Göre Dağılım</h2>
        <ResponsiveContainer width="100%" height={250}>
          <PieChart>
            <Pie data={getStatusPieData(notifications)} dataKey="value" nameKey="name" outerRadius={80} label>
              <Cell fill="#3b82f6" />
              <Cell fill="#10b981" />
            </Pie>
            <Tooltip />
            <Legend />
          </PieChart>
        </ResponsiveContainer>
      </div>
    </div>
  );
}

function getChartData(notifications: any[]) {
  const map = new Map<string, number>();
  notifications.forEach(n => {
    const date = new Date(n.createdAt).toLocaleDateString('tr-TR');
    map.set(date, (map.get(date) || 0) + 1);
  });
  return Array.from(map.entries()).map(([date, count]) => ({ date, count }));
}

function getStatusPieData(notifications: any[]) {
  const read = notifications.filter(n => n.status === 'Read').length;
  const unread = notifications.filter(n => n.status === 'Unread').length;
  return [
    { name: 'Okunmamış', value: unread },
    { name: 'Okunan', value: read },
  ];
}
