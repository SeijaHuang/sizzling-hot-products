"use client";

import getHotProductsAsync from "@/services/get-hot-products";
import { TopProductDaily } from "@/types/services/get-top-products";
import { useEffect, useState } from "react";

const TopProductsBoard = () => {
  const [products, setProducts] = useState<TopProductDaily[]>([]);

  useEffect(() => {
    const fetchTopProducts = async () => {
      const response = await getHotProductsAsync({
        startDate: "2024-04-21",
      });
      if (response.success && response.body) {
        setProducts(response.body.daily);
      }
    };
    fetchTopProducts();
  }, []);

  return (
    <div>
      {products.map((product: TopProductDaily) => (
        <div key={product.date}>
          <h3>{product.date}</h3>
          <p>{product.product.name}</p>
        </div>
      ))}
    </div>
  );
};

export default TopProductsBoard;
