export type OrderEntry = {
  id: string;
  quantity: number;
};

export type Order = {
  orderId: string;
  customerId: string;
  entries?: OrderEntry[];
  date: string;
  status: string;
};
