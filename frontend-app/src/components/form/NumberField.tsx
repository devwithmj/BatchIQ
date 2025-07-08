import { Input } from "@/components/ui/input";
import { Controller, Control, Path } from "react-hook-form";

import { FieldValues } from "react-hook-form";

type Props<T extends FieldValues> = {
  name: Path<T>;
  control: Control<T>;
  step?: string;
  placeholder?: string;
  label: string;
};

export function NumberField<T extends FieldValues>({
  name,
  control,
  step = ".01",
  placeholder,
  label,
}: Props<T>) {
  return (
    <Controller
      control={control}
      name={name}
      defaultValue={0 as any} 
      render={({ field, fieldState }) => (
        <div className="grid gap-1">
          <label className="text-sm font-medium">{label}</label>
          <Input
            type="number"
            step={step}
            placeholder={placeholder}
            value={field.value === 0 ? "" : field.value} // show empty box for 0
            onChange={(e) =>
              field.onChange(
                e.target.value === "" ? 0 : parseFloat(e.target.value)
              )
            }
          />
          {fieldState.error && (
            <p className="text-xs text-red-600">{fieldState.error.message}</p>
          )}
        </div>
      )}
    />
  );
}
