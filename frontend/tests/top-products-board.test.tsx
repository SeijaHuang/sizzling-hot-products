import { fireEvent, render, screen } from "@testing-library/react";
import { useQuery, UseQueryResult } from "@tanstack/react-query";
import { vi, describe, it, expect, beforeEach } from "vitest";
import TopProductsBoard from "@/components/TopProductsBoard";
import type {
  GetTopProductsResponse,
  TopProductDaily,
  TopProductPeriod,
} from "@/types/services/get-top-products";
import type { UseQueryOptions } from "@tanstack/react-query";

vi.mock("@/services/get-hot-products", () => ({
  default: vi.fn(),
}));

vi.mock("@tanstack/react-query", () => ({
  useQuery: vi.fn(),
}));

const mockUseQuery = vi.mocked(useQuery);

const mockQuery = (
  partial: Partial<UseQueryResult<GetTopProductsResponse, Error>>,
) =>
  mockUseQuery.mockReturnValue(
    partial as UseQueryResult<GetTopProductsResponse, Error>,
  );

const mockDailyData: TopProductDaily[] = [
  { date: "2026-04-21", product: { id: "1", name: "Power Drill" } },
  { date: "2026-04-22", product: { id: "2", name: "Paint Brush" } },
];

const mockPeriodData: TopProductPeriod = {
  startDate: "2026-04-21",
  endDate: "2026-04-23",
  product: { id: "3", name: "Hammer" },
};

describe("TopProductsBoard", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe("loading state", () => {
    it("renders skeleton and hides table while loading", () => {
      // Arrange
      mockQuery({ isLoading: true, data: undefined });

      // Act & Assert
      render(<TopProductsBoard />);

      expect(
        screen.getByTestId("top-products-board-skeleton"),
      ).toBeInTheDocument();
      expect(
        screen.queryByTestId("top-products-board-table"),
      ).not.toBeInTheDocument();
    });
  });

  describe("initial render", () => {
    it("renders title, flame icons, and date pickers", () => {
      // Arrange
      mockQuery({ isLoading: false, data: { daily: [] } });

      // Act & Assert
      render(<TopProductsBoard />);

      expect(screen.getByText("Top Sizzling Hot Products")).toBeInTheDocument();
      expect(screen.getAllByRole("img", { name: "Hot Product" })).toHaveLength(
        2,
      );
      expect(
        document.getElementById("top-products-board-start-date"),
      ).toBeInTheDocument();
      expect(
        document.getElementById("top-products-board-end-date"),
      ).toBeInTheDocument();
    });
  });

  describe("empty state", () => {
    it("shows no-results and hides table when data is empty or undefined", () => {
      // Arrange
      mockQuery({ isLoading: false, data: { daily: [] } });

      // Act & Assert
      const { unmount } = render(<TopProductsBoard />);

      expect(
        screen.getByTestId("top-products-board-no-results"),
      ).toBeInTheDocument();
      expect(
        screen.queryByTestId("top-products-board-table"),
      ).not.toBeInTheDocument();
      unmount();
    });
  });

  describe("data state", () => {
    it("renders table headers, daily rows, and formatted dates", () => {
      // Arrange
      mockQuery({
        isLoading: false,
        data: { daily: mockDailyData },
      });

      // Act & Assert
      render(<TopProductsBoard />);

      expect(screen.getByText("Date or Period")).toBeInTheDocument();
      expect(screen.getByText("Top Sizzling Hot Product")).toBeInTheDocument();
      expect(screen.getByText("Power Drill")).toBeInTheDocument();
      expect(screen.getByText("Paint Brush")).toBeInTheDocument();
      expect(screen.getByText("21/Apr/2026")).toBeInTheDocument();
      expect(screen.getByText("22/Apr/2026")).toBeInTheDocument();
    });

    it("renders period product and formatted date range", () => {
      // Arrange
      mockQuery({
        isLoading: false,
        data: { daily: mockDailyData, period: mockPeriodData },
      });

      // Act & Assert
      render(<TopProductsBoard />);

      expect(screen.getByText("Hammer")).toBeInTheDocument();
      const periodCell = screen
        .getAllByText(/21\/Apr\/2026/)
        .find((el) => el.textContent?.includes("23/Apr/2026"));
      expect(periodCell).toBeInTheDocument();
    });

    it("does not render a period row when period is undefined", () => {
      // Arrange
      mockQuery({
        isLoading: false,
        data: { daily: mockDailyData },
      });

      // Act & Assert
      render(<TopProductsBoard />);
      const rows = screen.getAllByRole("row");
      // 1 header row + 2 daily rows, no period row
      expect(rows).toHaveLength(3);
    });

    it("renders one row per unique product id", () => {
      // Arrange
      const daily: TopProductDaily[] = [
        { date: "2026-04-21", product: { id: "prod-1", name: "Power Drill" } },
        { date: "2026-04-22", product: { id: "prod-2", name: "Paint Brush" } },
        { date: "2026-04-23", product: { id: "prod-3", name: "Hammer" } },
      ];
      mockQuery({ isLoading: false, data: { daily } });

      // Act
      render(<TopProductsBoard />);

      // Assert
      const rows = screen.getAllByRole("row");
      // 1 header row + 3 daily rows
      expect(rows).toHaveLength(4);
      expect(screen.getByText("Power Drill")).toBeInTheDocument();
      expect(screen.getByText("Paint Brush")).toBeInTheDocument();
      expect(screen.getByText("Hammer")).toBeInTheDocument();
    });
  });

  describe("date selection", () => {
    it("renders new results after selecting a different start date", () => {
      // Arrange
      mockQuery({
        isLoading: false,
        data: { daily: mockDailyData, period: undefined },
      });
      render(<TopProductsBoard />);

      mockQuery({
        isLoading: false,
        data: {
          daily: [
            { date: "2026-04-20", product: { id: "4", name: "Lawn Mower" } },
          ],
          period: undefined,
        },
      });

      // Act
      fireEvent.click(
        document.getElementById("top-products-board-start-date")!,
      );
      fireEvent.click(
        screen.getByRole("button", { name: "Monday, April 20th, 2026" }),
      );

      // Assert
      expect(screen.getByText("Lawn Mower")).toBeInTheDocument();
      expect(screen.getByText("20/Apr/2026")).toBeInTheDocument();
      expect(screen.queryByText("Power Drill")).not.toBeInTheDocument();
    });

    it("renders new results after selecting a different end date", () => {
      // Arrange
      mockQuery({
        isLoading: false,
        data: { daily: mockDailyData, period: undefined },
      });
      render(<TopProductsBoard />);

      mockQuery({
        isLoading: false,
        data: {
          daily: [
            { date: "2026-04-21", product: { id: "1", name: "Power Drill" } },
            { date: "2026-04-22", product: { id: "5", name: "Screwdriver" } },
          ],
          period: undefined,
        },
      });

      // Act
      fireEvent.click(document.getElementById("top-products-board-end-date")!);
      fireEvent.click(
        screen.getByRole("button", { name: "Wednesday, April 22nd, 2026" }),
      );

      // Assert
      expect(screen.getByText("Screwdriver")).toBeInTheDocument();
    });
  });
});
