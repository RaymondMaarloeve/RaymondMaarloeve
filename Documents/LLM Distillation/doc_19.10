# Destylacja LLM do NPC

## Cel

Zoptymalizowanie rozmiarów i szybkości lokalnych modeli LLM minimalnym
kosztem utraty jakości.

## Co robimy

-   Bierzemy dużego nauczyciela i każemy generować mu przykładowe
    dialogi NPC.
-   Zbieramy pary **prompt--odpowiedź**. Prompt zawiera dane wejściowe
    (opis, dane, pytanie), a odpowiedź -- tekst NPC.
-   Uczymy mały model tak, aby jego odpowiedzi były podobne do tych z
    dużego modelu.
-   Finalnie mamy lekki model, który działa lokalnie.

## Frameworki

-   PyTorch, Hugging Face Transformers
-   Axolotl
-   Accelerate, LoRA, DeepSpeed

## Co sprawia, że działa dobrze

-   Temperatura \~2.0 → model uczy się stylu, a nie tylko słów.
-   Mieszane straty (KL + CrossEntropy) → zachowuje sens i styl
    wypowiedzi.
-   Różnorodne dane NPC → bardziej naturalne odpowiedzi.
-   Ujednolicony tokenizer → spójność między nauczycielem i studentem.

## Potencjalne problemy

-   Halucynacje.
-   Sztywny styl wypowiedzi modelu-studenta.
-   Utrata architektury odpowiedzi (np. przestaje mówić „po ludzku").
-   Za długie lub zbyt rozwlekłe odpowiedzi.

## Potencjalne zyski

Znacznie szybszy i mniejszy model pozwalający na prostszą implementację
w środowisku lokalnym.

------------------------------------------------------------------------

**Wykonali:**\
Kamil Włodarczyk\
Karol Rzepiński\
Łukasz Jastrzębski
