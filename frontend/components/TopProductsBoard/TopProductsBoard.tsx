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
} from "@/components/ui/table";
import FlameIcon from "./components/FlameIcon";
import { DatePicker } from "./components/DatePicker";
import { useState } from "react";
import { formatDateToString, formatStringToDate } from "@/utils/format-date";

const TopProductsBoard = () => {
  const [startDate, setStartDate] = useState<Date | undefined>(
    new Date("2026-04-21"),
  );
  const [endDate, setEndDate] = useState<Date | undefined>(
    new Date("2026-04-23"),
  );

  const { isLoading, data } = useQuery({
    queryKey: [SERVICE_KEYS.GET_HOT_PRODUCTS, startDate, endDate],
    queryFn: () =>
      getHotProductsAsync({
        startDate: formatDateToString(startDate),
        endDate: formatDateToString(endDate),
      }),
    select: (response) => response.body,
    enabled: !!startDate,
  });

  const renderContent = (): React.ReactNode => {
    if (isLoading) return <Skeleton className="w-full h-48" />;

    if (!data?.daily.length)
      return <div className="text-center text-xl">No hot products found.</div>;

    return (
      <div className="w-full overflow-x-auto rounded-md border">
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
                  {formatStringToDate(item.date)}
                </TableCell>
                <TableCell className="text-sm sm:text-base">
                  {item.product.name}
                </TableCell>
              </TableRow>
            ))}
            {data?.period && (
              <TableRow>
                <TableCell className="text-sm sm:text-base">
                  {formatStringToDate(data.period.startDate)} -{" "}
                  <br className="sm:hidden" />
                  {formatStringToDate(data.period.endDate)}
                </TableCell>
                <TableCell className="text-sm sm:text-base">
                  {data.period.product.name}
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>
    );
  };

  return (
    <div className="w-full text-neutral-800 flex flex-col items-center gap-6">
      <div className="flex items-center gap-4">
        <FlameIcon />
        <h1 className="text-lg sm:text-2xl font-bold text-center">
          Top Sizzling Hot Products
        </h1>
        <FlameIcon />
      </div>
      <div className="flex items-center gap-4">
        <DatePicker
          value={startDate}
          onChange={setStartDate}
          disabled={(date) => (endDate ? date > endDate : false)}
        />
        <span className="text-neutral-500">—</span>
        <DatePicker
          value={endDate}
          onChange={setEndDate}
          disabled={(date) => (startDate ? date < startDate : false)}
        />
      </div>
      {renderContent()}
    </div>
  );
};

export default TopProductsBoard;
