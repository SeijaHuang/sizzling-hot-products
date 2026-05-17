import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  GetTopProductsResponse,
  TopProductDaily,
} from "@/types/services/get-top-products";
import { formatStringToDate } from "@/utils/format-date";

interface TopProductsTableProps extends GetTopProductsResponse {
  id: string;
}

const TopProductsTable = ({ id, daily, period }: TopProductsTableProps) => {
  return (
    <div
      id={`${id}-table`}
      data-testid={`${id}-table`}
      className="w-full overflow-x-auto rounded-md border"
    >
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead className="text-primary text-sm sm:text-base font-bold w-36 sm:w-48">
              Date or Period
            </TableHead>
            <TableHead className="text-primary text-sm sm:text-base font-bold">
              Top Sizzling Hot Product
            </TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {daily?.map((item: TopProductDaily) => (
            <TableRow key={item.date}>
              <TableCell className="whitespace-nowrap text-sm sm:text-base">
                {formatStringToDate(item.date)}
              </TableCell>
              <TableCell className="text-sm sm:text-base">
                {item.product.name}
              </TableCell>
            </TableRow>
          ))}
          {period && (
            <TableRow>
              <TableCell className="text-sm sm:text-base">
                {formatStringToDate(period.startDate)} -{" "}
                <br className="sm:hidden" />
                {formatStringToDate(period.endDate)}
              </TableCell>
              <TableCell className="text-sm sm:text-base">
                {period.product.name}
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>
    </div>
  );
};

export default TopProductsTable;
