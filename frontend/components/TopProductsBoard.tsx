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
        <h1 className="text-lg sm:text-2xl font-bold text-center">
          Top Sizzling Hot Products
        </h1>
        <Image height={40} width={40} src="/flame.png" alt={"Hot Product"} />
      </div>

      {isLoading || !data ? (
        <Skeleton className="w-full h-48" />
      ) : (
        <div className="w-full overflow-x-auto rounded-md border">
          {" "}
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead className="text-neutral-800 whitespace-nowrap w-36 sm:w-48">
                  Date or Period
                </TableHead>
                <TableHead className="text-neutral-800">
                  Top Sizzling Hot Product
                </TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {data?.daily.map((item: TopProductDaily) => (
                <TableRow key={item.date}>
                  <TableCell className="whitespace-nowrap text-sm sm:text-base">
                    {formatDate(item.date)}
                  </TableCell>
                  <TableCell className="text-sm sm:text-base">
                    {item.product.name}
                  </TableCell>
                </TableRow>
              ))}
              {data?.period && (
                <TableRow>
                  <TableCell className="text-sm sm:text-base">
                    {formatDate(data.period.startDate)} -{" "}
                    <br className="sm:hidden" />
                    {formatDate(data.period.endDate)}
                  </TableCell>
                  <TableCell className="text-sm sm:text-base">
                    {data.period.product.name}
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </div>
      )}
    </div>
  );
};

export default TopProductsBoard;
