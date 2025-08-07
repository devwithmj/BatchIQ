"use client";

import { useState, useEffect, useRef } from "react";
import { Scan } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Combobox, ComboboxOption } from "@/components/ui/combobox";
import { cn } from "@/lib/utils";

interface ProductComboboxProps {
  options: ComboboxOption[];
  value?: string;
  onValueChange?: (value: string) => void;
  placeholder?: string;
  emptyText?: string;
  disabled?: boolean;
  className?: string;
}

export function ProductCombobox({
  options,
  value,
  onValueChange,
  placeholder = "Search product by name, brand, or scan barcode...",
  emptyText = "No product found",
  disabled = false,
  className,
}: ProductComboboxProps) {
  const [isScanning, setIsScanning] = useState(false);
  const [barcodeInput, setBarcodeInput] = useState("");
  const inputRef = useRef<HTMLInputElement>(null);

  // Handle barcode scanner input (usually very fast typing)
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      // If we're not actively focusing on other inputs and receive rapid key presses
      if (!isScanning && e.target === document.body) {
        setIsScanning(true);
        setBarcodeInput("");
      }

      if (isScanning) {
        if (e.key === "Enter") {
          // Barcode scan complete
          handleBarcodeComplete(barcodeInput);
          setIsScanning(false);
          setBarcodeInput("");
        } else if (e.key.length === 1) {
          // Add character to barcode
          setBarcodeInput(prev => prev + e.key);
        }
      }
    };

    // Auto-complete barcode after short delay (barcode scanners are fast)
    const timer = setTimeout(() => {
      if (isScanning && barcodeInput.length > 0) {
        handleBarcodeComplete(barcodeInput);
        setIsScanning(false);
        setBarcodeInput("");
      }
    }, 100);

    document.addEventListener("keydown", handleKeyDown);
    
    return () => {
      document.removeEventListener("keydown", handleKeyDown);
      clearTimeout(timer);
    };
  }, [isScanning, barcodeInput]);

  const handleBarcodeComplete = (code: string) => {
    // Find product by barcode/code
    const foundOption = options.find(option => 
      option.searchTerms?.some(term => 
        term.toLowerCase() === code.toLowerCase() ||
        term.includes(code)
      )
    );

    if (foundOption) {
      onValueChange?.(foundOption.value);
    }
  };

  const handleManualBarcodeEntry = () => {
    const code = prompt("Enter barcode manually:");
    if (code) {
      handleBarcodeComplete(code);
    }
  };

  return (
    <div className={cn("flex gap-2", className)}>
      <div className="flex-1">
        <Combobox
          options={options}
          value={value}
          onValueChange={onValueChange}
          placeholder={placeholder}
          emptyText={emptyText}
          disabled={disabled}
        />
      </div>
      <Button
        type="button"
        variant="outline"
        size="icon"
        onClick={handleManualBarcodeEntry}
        disabled={disabled}
        title="Enter barcode manually or use barcode scanner"
        className="shrink-0"
      >
        <Scan className="h-4 w-4" />
      </Button>
      
      {isScanning && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white p-6 rounded-lg shadow-lg">
            <div className="flex items-center space-x-2">
              <Scan className="h-5 w-5 animate-pulse" />
              <span>Scanning barcode...</span>
            </div>
            <div className="mt-2 text-sm text-gray-600">
              Scanned: {barcodeInput}
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
