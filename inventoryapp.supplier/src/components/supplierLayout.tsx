'use client';

import useLogout from '@/lib/logout';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import {
  LayoutDashboard,
  Package,
  LogOut,
  Bell,
  User,
  Menu,
  Sun,
  Moon,
  Settings
} from 'lucide-react';
import { useEffect, useState } from 'react';
import axiosInstance from '@/lib/axiosinstance';
import { AnimatePresence, motion } from 'framer-motion';

export default function Sidebar() {
  const logout = useLogout();
  const pathname = usePathname();
  const [unreadCount, setUnreadCount] = useState(0);
  const [userName, setUserName] = useState('');
  const [open, setOpen] = useState(false);
  const [theme, setTheme] = useState<'light' | 'dark'>('light');
  const [avatarUrl, setAvatarUrl] = useState('');
  const [menuOpen, setMenuOpen] = useState(false);

  useEffect(() => {
    axiosInstance.get('/auth/me')
      .then(res => {
        setUserName(res.data.user.firstName);
        setAvatarUrl(`https://ui-avatars.com/api/?name=${res.data.user.firstName}+${res.data.user.lastName}`);
        return axiosInstance.get(`/Notification/user-notifications/${res.data.user.id}`);
      })
      .then(res => {
        const data = res.data.data ?? res.data;
        setUnreadCount(data.filter((n: any) => n.status === 'Unread').length);
      })
      .catch(err => console.error('Sidebar verileri alınamadı:', err));

    const storedTheme = localStorage.getItem('theme') as 'light' | 'dark';
    if (storedTheme) {
      setTheme(storedTheme);
      document.documentElement.classList.toggle('dark', storedTheme === 'dark');
    }
  }, []);

  const toggleTheme = () => {
    const newTheme = theme === 'light' ? 'dark' : 'light';
    setTheme(newTheme);
    document.documentElement.classList.toggle('dark', newTheme === 'dark');
    localStorage.setItem('theme', newTheme);
  };

  const menuItems = [
    {
      name: 'Dashboard',
      href: '/dashboard',
      icon: <LayoutDashboard size={20} />, 
    },
    {
      name: 'Ürünlerim',
      href: '/products',
      icon: <Package size={20} />, 
    },
    {
      name: 'Profil',
      href: '/profile',
      icon: <User size={20} />, 
    },
    {
      name: 'Bildirimler',
      href: '/notifications',
      icon: (
        <div className="relative">
          <Bell size={20} />
          {unreadCount > 0 && (
            <span className="absolute -top-2 -right-2 bg-red-600 text-white text-[10px] px-1 rounded-full">
              {unreadCount}
            </span>
          )}
        </div>
      )
    },
  ];

  return (
    <>
      <button
        className="md:hidden fixed top-4 left-4 z-50 bg-blue-600 text-white p-2 rounded-full shadow-lg"
        onClick={() => setOpen(!open)}
      >
        <Menu size={20} />
      </button>

      <AnimatePresence>
        {open && (
          <motion.div
            className="fixed inset-0 bg-black/40 z-30 md:hidden"
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            onClick={() => setOpen(false)}
          />
        )}
      </AnimatePresence>

      <motion.aside
        className="fixed md:static z-40 top-0 left-0 h-screen w-64 bg-white dark:bg-gray-900 border-r border-gray-200 dark:border-gray-800 p-5 flex flex-col justify-between"
        initial={{ x: -300 }}
        animate={{ x: open || typeof window !== 'undefined' && window.innerWidth >= 768 ? 0 : -300 }}
        transition={{ type: 'spring', stiffness: 300, damping: 30 }}
      >
        <div>
          <h1 className="text-2xl font-bold text-blue-600 mb-8">Supplier Panel</h1>
          <nav className="space-y-2">
            {menuItems.map((item) => (
              <Link
                key={item.href}
                href={item.href}
                className={`flex items-center gap-3 px-4 py-2 rounded-lg transition ${
                  pathname === item.href
                    ? 'bg-blue-100 text-blue-700 font-semibold dark:bg-blue-950 dark:text-blue-400'
                    : 'text-gray-700 hover:bg-gray-100 dark:text-gray-300 dark:hover:bg-gray-800'
                }`}
                onClick={() => setOpen(false)}
              >
                {item.icon}
                <span className="text-sm font-medium">{item.name}</span>
              </Link>
            ))}
          </nav>
        </div>

        <div className="border-t pt-4 space-y-3 relative">
          <div className="flex items-center justify-between px-4 cursor-pointer" onClick={() => setMenuOpen(!menuOpen)}>
            <div className="flex items-center gap-3">
              <img src={avatarUrl} alt="Avatar" className="w-8 h-8 rounded-full" />
              <p className="text-sm text-gray-700 dark:text-gray-300 font-medium">{userName || 'Kullanıcı'}</p>
            </div>
            <button
              onClick={(e) => {
                e.stopPropagation();
                toggleTheme();
              }}
              className="text-gray-600 dark:text-gray-300 hover:text-blue-600 dark:hover:text-yellow-400"
              aria-label="Tema değiştir"
            >
              {theme === 'dark' ? <Sun size={18} /> : <Moon size={18} />}
            </button>
          </div>

          {menuOpen && (
            <motion.div
              initial={{ opacity: 0, y: 8 }}
              animate={{ opacity: 1, y: 0 }}
              exit={{ opacity: 0, y: 8 }}
              className="absolute bottom-16 left-4 right-4 bg-white dark:bg-gray-800 rounded-xl border border-gray-200 dark:border-gray-700 shadow-md p-4"
            >
              <Link href="/profile" className="flex items-center gap-2 text-sm text-gray-700 dark:text-gray-300 hover:underline">
                <User size={16} /> Profilim
              </Link>
              <Link href="/settings" className="flex items-center gap-2 text-sm text-gray-700 dark:text-gray-300 hover:underline mt-2">
                <Settings size={16} /> Ayarlar
              </Link>
              <button
                onClick={() => {
                  setOpen(false);
                  logout();
                }}
                className="flex items-center gap-2 text-sm text-red-500 hover:underline mt-2"
              >
                <LogOut size={16} /> Çıkış Yap
              </button>
            </motion.div>
          )}
        </div>
      </motion.aside>
    </>
  );
}
