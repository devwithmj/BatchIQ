"use client";

import * as React from "react";
import { Check, ChevronsUpDown, Search } from "lucide-react";
import { Command as CommandPrimitive } from "cmdk";
import * as PopoverPrimitive from "@radix-ui/react-popover";

import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";

const Command = React.forwardRef<
  React.ElementRef<typeof CommandPrimitive>,
  React.ComponentPropsWithoutRef<typeof CommandPrimitive>
>(({ className, ...props }, ref) => (
  <CommandPrimitive
    ref={ref}
    className={cn(
      "flex h-full w-full flex-col overflow-hidden rounded-md bg-popover text-popover-foreground",
      className
    )}
    {...props}
  />
));
Command.displayName = CommandPrimitive.displayName;

const CommandInput = React.forwardRef<
  React.ElementRef<typeof CommandPrimitive.Input>,
  React.ComponentPropsWithoutRef<typeof CommandPrimitive.Input>
>(({ className, ...props }, ref) => (
  <div className="flex items-center border-b px-3" cmdk-input-wrapper="">
    <Search className="mr-2 h-4 w-4 shrink-0 opacity-50" />
    <CommandPrimitive.Input
      ref={ref}
      className={cn(
        "flex h-10 w-full rounded-md bg-transparent py-3 text-sm outline-none placeholder:text-muted-foreground disabled:cursor-not-allowed disabled:opacity-50",
        className
      )}
      {...props}
    />
  </div>
));
CommandInput.displayName = CommandPrimitive.Input.displayName;

const CommandList = React.forwardRef<
  React.ElementRef<typeof CommandPrimitive.List>,
  React.ComponentPropsWithoutRef<typeof CommandPrimitive.List>
>(({ className, ...props }, ref) => (
  <CommandPrimitive.List
    ref={ref}
    className={cn("max-h-[300px] overflow-y-auto overflow-x-hidden", className)}
    {...props}
  />
));
CommandList.displayName = CommandPrimitive.List.displayName;

const CommandEmpty = React.forwardRef<
  React.ElementRef<typeof CommandPrimitive.Empty>,
  React.ComponentPropsWithoutRef<typeof CommandPrimitive.Empty>
>((props, ref) => (
  <CommandPrimitive.Empty
    ref={ref}
    className="py-6 text-center text-sm"
    {...props}
  />
));
CommandEmpty.displayName = CommandPrimitive.Empty.displayName;

const CommandGroup = React.forwardRef<
  React.ElementRef<typeof CommandPrimitive.Group>,
  React.ComponentPropsWithoutRef<typeof CommandPrimitive.Group>
>(({ className, ...props }, ref) => (
  <CommandPrimitive.Group
    ref={ref}
    className={cn(
      "overflow-hidden p-1 text-foreground [&_[cmdk-group-heading]]:px-2 [&_[cmdk-group-heading]]:py-1.5 [&_[cmdk-group-heading]]:text-xs [&_[cmdk-group-heading]]:font-medium [&_[cmdk-group-heading]]:text-muted-foreground",
      className
    )}
    {...props}
  />
));
CommandGroup.displayName = CommandPrimitive.Group.displayName;

const CommandItem = React.forwardRef<
  React.ElementRef<typeof CommandPrimitive.Item>,
  React.ComponentPropsWithoutRef<typeof CommandPrimitive.Item>
>(({ className, ...props }, ref) => (
  <CommandPrimitive.Item
    ref={ref}
    className={cn(
      "relative flex cursor-default select-none items-center rounded-sm px-2 py-1.5 text-sm outline-none aria-selected:bg-accent aria-selected:text-accent-foreground data-[disabled=true]:pointer-events-none data-[disabled=true]:opacity-50",
      className
    )}
    {...props}
  />
));
CommandItem.displayName = CommandPrimitive.Item.displayName;

const Popover = PopoverPrimitive.Root;
const PopoverTrigger = PopoverPrimitive.Trigger;
const PopoverContent = React.forwardRef<
  React.ElementRef<typeof PopoverPrimitive.Content>,
  React.ComponentPropsWithoutRef<typeof PopoverPrimitive.Content>
>(({ className, align = "start", sideOffset = 4, ...props }, ref) => (
  <PopoverPrimitive.Portal>
    <PopoverPrimitive.Content
      ref={ref}
      align={align}
      sideOffset={sideOffset}
      className={cn(
        "z-50 w-72 rounded-md border bg-popover p-0 text-popover-foreground shadow-md outline-none data-[state=open]:animate-in data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=open]:fade-in-0 data-[state=closed]:zoom-out-95 data-[state=open]:zoom-in-95 data-[side=bottom]:slide-in-from-top-2 data-[side=left]:slide-in-from-right-2 data-[side=right]:slide-in-from-left-2 data-[side=top]:slide-in-from-bottom-2",
        className
      )}
      {...props}
    />
  </PopoverPrimitive.Portal>
));
PopoverContent.displayName = PopoverPrimitive.Content.displayName;

// Custom hook for debounced search
function useDebounce<T>(value: T, delay: number): T {
  const [debouncedValue, setDebouncedValue] = React.useState<T>(value);

  React.useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedValue(value);
    }, delay);

    return () => {
      clearTimeout(handler);
    };
  }, [value, delay]);

  return debouncedValue;
}

// Virtual list item component
const VirtualListItem = React.memo(({ 
  option, 
  isSelected, 
  onSelect 
}: { 
  option: ComboboxOption; 
  isSelected: boolean; 
  onSelect: () => void;
}) => (
  <CommandItem
    value={option.value}
    onSelect={onSelect}
  >
    <Check
      className={cn(
        "mr-2 h-4 w-4",
        isSelected ? "opacity-100" : "opacity-0"
      )}
    />
    <div className="flex flex-col min-w-0 flex-1">
      <span className="truncate">{option.label}</span>
      {option.subtitle && (
        <span className="text-xs text-muted-foreground truncate">
          {option.subtitle}
        </span>
      )}
    </div>
  </CommandItem>
));
VirtualListItem.displayName = "VirtualListItem";

export interface ComboboxOption {
  value: string;
  label: string;
  searchTerms?: string[];
  subtitle?: string;
}

interface OptimizedComboboxProps {
  options: ComboboxOption[];
  value?: string;
  onValueChange?: (value: string) => void;
  placeholder?: string;
  emptyText?: string;
  disabled?: boolean;
  className?: string;
  searchDelay?: number; // Debounce delay in ms
  maxVisibleItems?: number; // Limit visible items for performance
}

export function OptimizedCombobox({
  options,
  value,
  onValueChange,
  placeholder = "Select option...",
  emptyText = "No option found.",
  disabled = false,
  className,
  searchDelay = 300,
  maxVisibleItems = 100,
}: OptimizedComboboxProps) {
  const [open, setOpen] = React.useState(false);
  const [searchTerm, setSearchTerm] = React.useState("");
  const debouncedSearchTerm = useDebounce(searchTerm, searchDelay);

  // Memoized filtered options with performance optimization
  const filteredOptions = React.useMemo(() => {
    if (!debouncedSearchTerm.trim()) {
      return options.slice(0, maxVisibleItems); // Limit initial display
    }

    const searchLower = debouncedSearchTerm.toLowerCase();
    const filtered = options.filter((option) => {
      // Primary search on label (fastest)
      if (option.label.toLowerCase().includes(searchLower)) {
        return true;
      }
      
      // Secondary search on search terms
      if (option.searchTerms) {
        return option.searchTerms.some(term => 
          term.toLowerCase().includes(searchLower)
        );
      }
      
      return false;
    });

    return filtered.slice(0, maxVisibleItems); // Limit results for performance
  }, [options, debouncedSearchTerm, maxVisibleItems]);

  // Memoized selected option
  const selectedOption = React.useMemo(() => 
    options.find((option) => option.value === value),
    [options, value]
  );

  // Reset search when closed
  React.useEffect(() => {
    if (!open) {
      setSearchTerm("");
    }
  }, [open]);

  const handleSelect = React.useCallback((selectedValue: string) => {
    onValueChange?.(selectedValue);
    setOpen(false);
  }, [onValueChange]);

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger asChild>
        <Button
          variant="outline"
          role="combobox"
          aria-expanded={open}
          className={cn("w-full justify-between", className)}
          disabled={disabled}
        >
          <span className="truncate">
            {selectedOption ? selectedOption.label : placeholder}
          </span>
          <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-full p-0" style={{ width: "var(--radix-popover-trigger-width)" }}>
        <Command shouldFilter={false}>
          <CommandInput 
            placeholder={`Search ${placeholder.toLowerCase()}...`}
            value={searchTerm}
            onValueChange={setSearchTerm}
          />
          <CommandEmpty>{emptyText}</CommandEmpty>
          <CommandGroup>
            <CommandList>
              {filteredOptions.map((option) => (
                <VirtualListItem
                  key={option.value}
                  option={option}
                  isSelected={value === option.value}
                  onSelect={() => handleSelect(option.value)}
                />
              ))}
              {options.length > maxVisibleItems && !debouncedSearchTerm && (
                <div className="px-2 py-1.5 text-xs text-muted-foreground text-center">
                  Showing {Math.min(maxVisibleItems, options.length)} of {options.length} items
                  <br />
                  Type to search for more...
                </div>
              )}
            </CommandList>
          </CommandGroup>
        </Command>
      </PopoverContent>
    </Popover>
  );
}

export {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
};
