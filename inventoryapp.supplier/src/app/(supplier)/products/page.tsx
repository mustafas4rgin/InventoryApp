'use client';

import { useEffect, useState } from 'react';
import axiosInstance from '@/lib/axiosinstance';
import toast from 'react-hot-toast';
import { useSearchParams } from 'next/navigation';

// Type tanımları

type Product = {
  id: number;
  name: string;
  price: number;
  stock: number;
  category: {
    id: number;
    name: string;
  } | null;
};

type Category = {
  id: number;
  name: string;
};

type User = {
  id: number;
  name: string;
  email: string;
  supplierId: number;
};

export default function ProductsPage() {

  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingProduct, setEditingProduct] = useState<Product | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [name, setName] = useState('');
  const [price, setPrice] = useState('');
  const [stock, setStock] = useState('');
  const [categoryId, setCategoryId] = useState('');
  const [categories, setCategories] = useState<Category[]>([]);
  const [user, setUser] = useState<User | null>(null);
  const [showDeleted, setShowDeleted] = useState(false);

  const searchParams = useSearchParams();
const isLowStock = searchParams.get('filter') === 'lowstock';

const fetchProducts = () => {
  const endpoint = showDeleted ? '/product/owndeleted' : '/product/own';
  axiosInstance.get(endpoint)
    .then(res => {
      const productsList = res.data;
      const validProducts = Array.isArray(productsList) ? productsList : [];
      const filtered = isLowStock ? validProducts.filter(p => p.stock < 10) : validProducts;
      setProducts(filtered);
    })
    .catch(err => {
      console.error('Ürünler alınamadı:', err);
      toast.error('Ürünler alınamadı');
      setProducts([]); 
    })
    .finally(() => setLoading(false));
};

useEffect(() => {
  fetchProducts();
  axiosInstance.get('/category/getall').then(res => setCategories(res.data));
  axiosInstance.get('/auth/me').then(res => setUser(res.data));
}, [showDeleted, searchParams]);

const filteredProducts = products.filter(p =>
  p.name.toLowerCase().includes(searchTerm.toLowerCase())
);


  const openCreateModal = () => {
    setEditingProduct(null);
    setName('');
    setPrice('');
    setStock('');
    setCategoryId('');
    setIsModalOpen(true);
  };

  const openEditModal = (product: Product) => {
    setEditingProduct(product);
    setName(product.name);
    setPrice(product.price.toString());
    setStock(product.stock.toString());
    setCategoryId(product.category?.id.toString() ?? '');
    setIsModalOpen(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!user) return;

    const payload = {
      name,
      price: parseFloat(price),
      stock: parseInt(stock),
      categoryId: parseInt(categoryId),
      supplierId: user.supplierId,
    };

    try {
      if (editingProduct) {
        await axiosInstance.put(`/product/update/${editingProduct.id}`, payload);
        toast.success('Ürün güncellendi');
      } else {
        await axiosInstance.post('/product/add', payload);
        toast.success('Ürün eklendi');
      }
      setIsModalOpen(false);
      fetchProducts();
    } catch (err) {
      console.error(err);
      toast.error('İşlem başarısız');
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('Bu ürünü silmek istediğinize emin misiniz?')) return;
    try {
      await axiosInstance.delete(`/product/delete/${id}`);
      toast.success('Ürün silindi');
      fetchProducts();
    } catch (err) {
      console.error(err);
      toast.error('Silme işlemi başarısız');
    }
  };

  const handleRestore = async (id: number) => {
    try {
      await axiosInstance.put(`/product/restore/${id}`, {
        isDeleted: false,
      });
      toast.success('Ürün geri yüklendi');
      fetchProducts();
    } catch (err) {
      console.error(err);
      toast.error('Geri alma işlemi başarısız');
    }
  };

  return (
    <div className="p-6 min-h-screen bg-gray-50 dark:bg-[#0f172a]">
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4 mb-6">
        <h1 className="text-3xl font-bold text-gray-900 dark:text-white">{showDeleted ? 'Silinen Ürünler' : 'Ürünlerim'}</h1>
        <div className="flex gap-2 w-full md:w-auto flex-wrap">
          <input
            type="text"
            placeholder="Ürün ara..."
            value={searchTerm}
            onChange={e => setSearchTerm(e.target.value)}
            className="w-full md:w-64 p-2 border rounded bg-white dark:bg-gray-800 dark:border-gray-700 dark:text-white"
          />
          <button
            onClick={openCreateModal}
            className="bg-blue-600 hover:bg-blue-700 text-white font-semibold px-4 rounded"
          >
            + Ürün Ekle
          </button>
          <button
            onClick={() => setShowDeleted(!showDeleted)}
            className="bg-gray-500 hover:bg-gray-600 text-white font-semibold px-4 rounded"
          >
            {showDeleted ? 'Aktifleri Göster' : 'Silinenleri Göster'}
          </button>
        </div>
      </div>

      {loading ? (
        <p className="text-gray-500 dark:text-gray-400">Yükleniyor...</p>
      ) : filteredProducts.length === 0 ? (
        <p className="text-gray-600 dark:text-gray-300">Hiç eşleşen ürün yok.</p>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          {filteredProducts.map(product => (
            <div
              key={product.id}
              className="bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-xl p-5 shadow flex flex-col justify-between"
            >
              <div>
                <h3 className="text-lg font-semibold text-gray-900 dark:text-white mb-1">
                  {product.name}
                </h3>
                <p className="text-sm text-gray-600 dark:text-gray-300">Kategori: {product.category?.name ?? 'Bilinmiyor'}</p>
                <p className="text-sm text-gray-600 dark:text-gray-300">Fiyat: ₺{product.price}</p>
                <p className="text-sm text-gray-600 dark:text-gray-300">Stok: {product.stock}</p>
              </div>
              {!showDeleted ? (
                <div className="flex gap-2 mt-4">
                  <button
                    onClick={() => openEditModal(product)}
                    className="flex-1 px-4 py-2 bg-yellow-500 text-white rounded hover:bg-yellow-600 transition"
                  >
                    Güncelle
                  </button>
                  <button
                    onClick={() => handleDelete(product.id)}
                    className="flex-1 px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700 transition"
                  >
                    Sil
                  </button>
                </div>
              ) : (
                <div className="flex mt-4">
                  <button
                    onClick={() => handleRestore(product.id)}
                    className="w-full px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700 transition"
                  >
                    Geri Al
                  </button>
                </div>
              )}
            </div>
          ))}
        </div>
      )}

      {isModalOpen && (
        <div className="fixed inset-0 z-50 bg-white/30 backdrop-blur-sm flex items-center justify-center transition-opacity">
          <div className="bg-white dark:bg-gray-900 rounded-xl shadow-xl p-6 w-full max-w-md animate-fade-in-up">
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-xl font-bold text-gray-800 dark:text-white flex items-center gap-2">
                <span>{editingProduct ? '✏️' : '➕'}</span>
                {editingProduct ? 'Ürünü Güncelle' : 'Yeni Ürün Ekle'}
              </h2>
              <button
                onClick={() => setIsModalOpen(false)}
                className="text-gray-500 hover:text-red-600 transition text-xl"
              >
                ×
              </button>
            </div>

            <form onSubmit={handleSubmit} className="space-y-4">
              <input
                type="text"
                placeholder="Ürün Adı"
                value={name}
                onChange={e => setName(e.target.value)}
                className="w-full p-2 border rounded bg-white dark:bg-gray-800 dark:border-gray-700 dark:text-white"
                required
              />
              <input
                type="number"
                placeholder="Fiyat"
                value={price}
                onChange={e => setPrice(e.target.value)}
                className="w-full p-2 border rounded bg-white dark:bg-gray-800 dark:border-gray-700 dark:text-white"
                required
              />
              <input
                type="number"
                placeholder="Stok"
                value={stock}
                onChange={e => setStock(e.target.value)}
                className="w-full p-2 border rounded bg-white dark:bg-gray-800 dark:border-gray-700 dark:text-white"
                required
              />
              <select
                value={categoryId}
                onChange={e => setCategoryId(e.target.value)}
                className="w-full p-2 border rounded bg-white dark:bg-gray-800 dark:border-gray-700 dark:text-white"
                required
              >
                <option value="">Kategori Seç</option>
                {categories.map(cat => (
                  <option key={cat.id} value={cat.id}>{cat.name}</option>
                ))}
              </select>
              <div className="flex justify-end gap-2">
                <button
                  type="button"
                  onClick={() => setIsModalOpen(false)}
                  className="px-4 py-2 bg-gray-200 dark:bg-gray-700 rounded text-gray-800 dark:text-gray-100 hover:bg-gray-300 dark:hover:bg-gray-600 transition"
                >
                  İptal
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition"
                >
                  {editingProduct ? 'Güncelle' : 'Kaydet'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}