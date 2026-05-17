import { Product } from "../product";

export type TopProductDaily = {
  date: string;
  product: Product;
};

export type TopProductPeriod = {
  startDate: string;
  endDate: string;
  product: Product;
};

export type GetTopProductsResponse = {
  daily: TopProductDaily[];
  period: TopProductPeriod;
};
