import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useNavigate, Link } from "@tanstack/react-router";
import { ProtectedRoute } from "@/features/auth/components/ProtectedRoute.tsx";
import { useBasket } from "@/features/basket/api/basket.queries.ts";
import { usePlaceOrder } from "../api/order.mutations.ts";
import { Button } from "@/shared/components/ui/Button.tsx";
import { Input } from "@/shared/components/ui/Input.tsx";
import { Textarea } from "@/shared/components/ui/Textarea.tsx";
import { EmptyState } from "@/shared/components/ui/EmptyState.tsx";
import { formatCurrency } from "@/shared/lib/format.ts";

const addressSchema = z.object({
  street: z.string().min(1, "Adres zorunludur"),
  city: z.string().min(1, "Şehir zorunludur"),
  state: z.string().min(1, "İlçe/Bölge zorunludur"),
  country: z.string().min(1, "Ülke zorunludur"),
  zipCode: z.string().min(1, "Posta kodu zorunludur"),
});

const paymentSchema = z.object({
  cardNumber: z
    .string()
    .min(1, "Kart numarası zorunludur")
    .transform((v) => v.replace(/\s/g, ""))
    .pipe(
      z
        .string()
        .regex(/^\d{16}$/, "Kart numarası 16 haneli olmalıdır"),
    ),
  expiryMonth: z
    .string()
    .min(1, "Ay zorunludur")
    .regex(/^(0[1-9]|1[0-2])$/, "Geçersiz ay (01-12)"),
  expiryYear: z
    .string()
    .min(1, "Yıl zorunludur")
    .regex(/^\d{2}$/, "Geçersiz yıl"),
  cvv: z
    .string()
    .min(1, "CVV zorunludur")
    .regex(/^\d{3,4}$/, "CVV 3-4 haneli olmalıdır"),
  cardHolderName: z.string().min(1, "Kart sahibi adı zorunludur"),
});

type AddressFormData = z.infer<typeof addressSchema>;
type PaymentFormData = z.input<typeof paymentSchema>;
type Step = "address" | "payment";

function formatCardNumber(value: string): string {
  const digits = value.replace(/\D/g, "").slice(0, 16);
  return digits.replace(/(\d{4})(?=\d)/g, "$1 ");
}

function CheckoutContent() {
  const { data: basket } = useBasket();
  const placeOrder = usePlaceOrder();
  const navigate = useNavigate();
  const [step, setStep] = useState<Step>("address");
  const [addressData, setAddressData] = useState<AddressFormData | null>(null);

  const addressForm = useForm<AddressFormData>({
    resolver: zodResolver(addressSchema) as never,
    defaultValues: { country: "Türkiye" },
  });

  const paymentForm = useForm<PaymentFormData>({
    resolver: zodResolver(paymentSchema) as never,
    defaultValues: {
      cardNumber: "4242 4242 4242 4242",
      expiryMonth: "12",
      expiryYear: "28",
      cvv: "123",
      cardHolderName: "Test Kullanici",
    },
  });

  const onAddressSubmit = (data: AddressFormData) => {
    setAddressData(data);
    setStep("payment");
  };

  const onPaymentSubmit = () => {
    if (!basket?.items.length || !addressData) return;

    placeOrder.mutate(
      {
        ...addressData,
        items: basket.items.map((item) => ({
          productId: item.productId,
          productName: item.productName,
          unitPrice: item.unitPrice,
          quantity: item.quantity,
        })),
      },
      {
        onSuccess: () => {
          navigate({ to: "/orders" });
        },
      },
    );
  };

  if (!basket?.items.length) {
    return (
      <EmptyState
        title="Sepetiniz boş"
        description="Sipariş oluşturmak için önce sepetinize ürün ekleyin"
        action={
          <Button asChild>
            <Link to="/products">Ürünlere Git</Link>
          </Button>
        }
      />
    );
  }

  const steps = [
    { label: "Sepet", done: true },
    { label: "Adres", done: step === "payment", active: step === "address" },
    { label: "Ödeme", done: false, active: step === "payment" },
    { label: "Onay", done: false },
  ];

  return (
    <div className="space-y-8">
      {/* Step indicator */}
      <div className="flex items-center justify-center gap-0">
        {steps.map((s, i) => (
          <div key={s.label} className="flex items-center">
            <div className="flex flex-col items-center">
              <div
                className={`h-8 w-8 rounded-full flex items-center justify-center text-xs font-bold ${
                  s.done
                    ? "bg-success text-white"
                    : s.active
                      ? "bg-primary text-primary-foreground"
                      : "bg-muted text-muted-foreground"
                }`}
              >
                {s.done ? "✓" : i + 1}
              </div>
              <span
                className={`text-xs mt-1.5 ${s.active ? "text-primary font-medium" : "text-muted-foreground"}`}
              >
                {s.label}
              </span>
            </div>
            {i < steps.length - 1 && (
              <div
                className={`w-12 sm:w-20 h-0.5 mx-2 mb-5 ${
                  s.done ? "bg-success" : "bg-border"
                }`}
              />
            )}
          </div>
        ))}
      </div>

      <div className="grid lg:grid-cols-3 gap-8">
        <div className="lg:col-span-2 space-y-6">
          {/* Address Step */}
          {step === "address" && (
            <form
              onSubmit={addressForm.handleSubmit(onAddressSubmit)}
              className="space-y-6"
            >
              <div className="rounded-xl border border-border bg-card p-6 space-y-5">
                <h2 className="text-lg font-semibold">Teslimat Adresi</h2>

                <div>
                  <label className="text-sm font-medium mb-1.5 block">
                    Adres
                  </label>
                  <Textarea
                    {...addressForm.register("street")}
                    placeholder="Mahalle, sokak, bina no, daire no..."
                    error={addressForm.formState.errors.street?.message}
                    rows={3}
                  />
                </div>

                <div className="grid sm:grid-cols-2 gap-4">
                  <div>
                    <label className="text-sm font-medium mb-1.5 block">
                      Şehir
                    </label>
                    <Input
                      {...addressForm.register("city")}
                      placeholder="İstanbul"
                      error={addressForm.formState.errors.city?.message}
                    />
                  </div>
                  <div>
                    <label className="text-sm font-medium mb-1.5 block">
                      İlçe/Bölge
                    </label>
                    <Input
                      {...addressForm.register("state")}
                      placeholder="Kadıköy"
                      error={addressForm.formState.errors.state?.message}
                    />
                  </div>
                </div>

                <div className="grid sm:grid-cols-2 gap-4">
                  <div>
                    <label className="text-sm font-medium mb-1.5 block">
                      Posta Kodu
                    </label>
                    <Input
                      {...addressForm.register("zipCode")}
                      placeholder="34000"
                      error={addressForm.formState.errors.zipCode?.message}
                    />
                  </div>
                  <div>
                    <label className="text-sm font-medium mb-1.5 block">
                      Ülke
                    </label>
                    <Input
                      {...addressForm.register("country")}
                      placeholder="Türkiye"
                      error={addressForm.formState.errors.country?.message}
                    />
                  </div>
                </div>
              </div>

              <Button type="submit" size="lg" className="w-full shadow-md">
                Ödeme Adımına Geç
              </Button>
            </form>
          )}

          {/* Payment Step */}
          {step === "payment" && (
            <form
              onSubmit={paymentForm.handleSubmit(onPaymentSubmit)}
              className="space-y-6"
            >
              <div className="rounded-xl border border-border bg-card p-6 space-y-5">
                <div className="flex items-center justify-between">
                  <h2 className="text-lg font-semibold">Ödeme Bilgileri</h2>
                  <span className="inline-flex items-center gap-1.5 rounded-full bg-amber-100 px-3 py-1 text-xs font-medium text-amber-800 dark:bg-amber-900/30 dark:text-amber-400">
                    Test Ödeme
                  </span>
                </div>

                <div>
                  <label className="text-sm font-medium mb-1.5 block">
                    Kart Numarası
                  </label>
                  <Input
                    {...paymentForm.register("cardNumber", {
                      onChange: (e) => {
                        e.target.value = formatCardNumber(e.target.value);
                      },
                    })}
                    placeholder="0000 0000 0000 0000"
                    maxLength={19}
                    inputMode="numeric"
                    error={paymentForm.formState.errors.cardNumber?.message}
                  />
                </div>

                <div className="grid grid-cols-3 gap-4">
                  <div>
                    <label className="text-sm font-medium mb-1.5 block">
                      Ay
                    </label>
                    <Input
                      {...paymentForm.register("expiryMonth")}
                      placeholder="MM"
                      maxLength={2}
                      inputMode="numeric"
                      error={paymentForm.formState.errors.expiryMonth?.message}
                    />
                  </div>
                  <div>
                    <label className="text-sm font-medium mb-1.5 block">
                      Yıl
                    </label>
                    <Input
                      {...paymentForm.register("expiryYear")}
                      placeholder="YY"
                      maxLength={2}
                      inputMode="numeric"
                      error={paymentForm.formState.errors.expiryYear?.message}
                    />
                  </div>
                  <div>
                    <label className="text-sm font-medium mb-1.5 block">
                      CVV
                    </label>
                    <Input
                      {...paymentForm.register("cvv")}
                      placeholder="123"
                      maxLength={4}
                      inputMode="numeric"
                      type="password"
                      error={paymentForm.formState.errors.cvv?.message}
                    />
                  </div>
                </div>

                <div>
                  <label className="text-sm font-medium mb-1.5 block">
                    Kart Sahibi
                  </label>
                  <Input
                    {...paymentForm.register("cardHolderName")}
                    placeholder="Ad Soyad"
                    error={
                      paymentForm.formState.errors.cardHolderName?.message
                    }
                  />
                </div>

                <p className="text-xs text-muted-foreground">
                  Bu bir test ödeme formudur. Gerçek kart bilgisi
                  işlenmemektedir.
                </p>
              </div>

              <div className="flex gap-3">
                <Button
                  type="button"
                  variant="outline"
                  size="lg"
                  className="flex-1"
                  onClick={() => setStep("address")}
                >
                  Geri
                </Button>
                <Button
                  type="submit"
                  size="lg"
                  className="flex-[2] shadow-md"
                  disabled={placeOrder.isPending}
                >
                  {placeOrder.isPending
                    ? "Sipariş oluşturuluyor..."
                    : "Siparişi Onayla"}
                </Button>
              </div>
            </form>
          )}
        </div>

        {/* Order summary */}
        <div className="lg:col-span-1">
          <div className="sticky top-24 rounded-xl border border-border bg-card p-6 space-y-4">
            <h2 className="text-lg font-semibold">Sipariş Özeti</h2>
            <div className="space-y-3 max-h-[300px] overflow-y-auto">
              {basket.items.map((item) => (
                <div
                  key={item.productId}
                  className="flex justify-between text-sm gap-2"
                >
                  <span className="text-muted-foreground truncate">
                    {item.productName}{" "}
                    <span className="text-foreground">×{item.quantity}</span>
                  </span>
                  <span className="whitespace-nowrap font-medium">
                    {formatCurrency(item.lineTotal, item.currency)}
                  </span>
                </div>
              ))}
            </div>
            <hr className="border-border" />
            <div className="flex justify-between text-sm text-muted-foreground">
              <span>Kargo</span>
              <span className="text-success font-medium">Ücretsiz</span>
            </div>
            <div className="flex justify-between font-bold text-lg">
              <span>Toplam</span>
              <span className="text-primary">
                {formatCurrency(basket.totalPrice)}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default function CheckoutPage() {
  return (
    <ProtectedRoute>
      <div className="space-y-6 animate-fade-in">
        <h1 className="text-2xl font-bold">Sipariş Oluştur</h1>
        <CheckoutContent />
      </div>
    </ProtectedRoute>
  );
}
