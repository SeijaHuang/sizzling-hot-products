"use client";

import getHotProductsAsync from "@/services/get-hot-products";
import { SERVICE_KEYS } from "@/types/common/service-keys";
import { TopProductDaily } from "@/types/services/get-top-products";
import { useQuery } from "@tanstack/react-query";
import { Skeleton } from "@/components/ui/skeleton";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "./ui/table";
import { formatDate } from "@/utils/format-date";
import Image from "next/image";

const TopProductsBoard = () => {
  const { isLoading, data } = useQuery({
    queryKey: [SERVICE_KEYS.GET_HOT_PRODUCTS],
    queryFn: () => getHotProductsAsync({ startDate: "2026-04-21" }),
    select: (response) => response.body,
  });

  return (
    <div className="w-full text-neutral-800 flex flex-col items-center gap-6">
      <div className="flex items-center gap-4">
        <Image height={40} width={40} src="/flame.png" alt={"Hot Product"} />
        <h1 className="text-2xl font-bold text-center">
          Top Sizzling Hot Products
        </h1>
        <Image height={40} width={40} src="/flame.png" alt={"Hot Product"} />
      </div>

      <Table>
        <TableHeader>
          <TableRow>
            <TableHead className="text-neutral-800">Date or Period</TableHead>
            <TableHead className="text-neutral-800">
              Top Sizzling Hot Product
            </TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {isLoading || !data ? (
            Array.from({ length: 3 }).map((_, i) => (
              <TableRow key={i}>
                <TableCell>
                  <Skeleton className="h-4 w-28" />
                </TableCell>
                <TableCell>
                  <Skeleton className="h-4 w-48" />
                </TableCell>
              </TableRow>
            ))
          ) : (
            <>
              {data?.daily.map((item: TopProductDaily) => (
                <TableRow key={item.date}>
                  <TableCell>{formatDate(item.date)}</TableCell>
                  <TableCell>{item.product.name}</TableCell>
                </TableRow>
              ))}
              {data?.period && (
                <TableRow>
                  <TableCell>
                    {formatDate(data.period.startDate)} -{" "}
                    {formatDate(data.period.endDate)}
                  </TableCell>
                  <TableCell className="flex items-center gap-2">
                    {data.period.product.name}
                  </TableCell>
                </TableRow>
              )}
            </>
          )}
        </TableBody>
      </Table>
    </div>
  );
};

export default TopProductsBoard;
