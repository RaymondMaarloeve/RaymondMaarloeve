# Sprawozdanie z analizy procesu destylacji

## 1. Cel
Celem analizy było zidentyfikowanie przyczyn niskiej jakości modelu studenta, mimo poprawnych promptów wejściowych przekazywanych do modelu nauczyciela.

---

## 2. Zaobserwowane problemy

### 2.1. Input (prompt) został włączony do procesu uczenia
W implementacji pełna sekwencja:

<prompt> <odpowiedź nauczyciela>

była traktowana jako jedna sekwencja treningowa, a model student uczył się odtwarzać **zarówno prompt, jak i odpowiedź**.  
Brak maskowania części promptu spowodował:

- mieszanie gradientów,
- pogorszenie jakości generowania,
- tendencję modelu do przepisywania lub streszczania inputu,
- niestabilne i nielogiczne odpowiedzi.

Model zamiast „odpowiadać na prompt” — uczył się „przepisywać całą sekwencję”.

---

### 2.2. Ograniczenie długości (`max_length = 512`) obcinało dane
W procesie generowania oraz destylacji zastosowano:
max_length = 512

co prowadziło do obcinania dłuższych odpowiedzi nauczyciela.  
Skutki:

- końcówki odpowiedzi były ucinane,
- model student trenował na niekompletnych sekwencjach,
- traciła się strukturalna spójność danych,
- model generował urwane lub chaotyczne odpowiedzi.

---

## 3. Wnioski
Niska jakość zdestylowanego modelu wynikała przede wszystkim z:

1. Włączenia całego inputu do procesu uczenia, zamiast trenowania jedynie na części odpowiedzi.  
2. Zbyt niskiego ograniczenia długości (`max_length`), które deformowało dane generowane przez model nauczyciela.

Oba czynniki prowadziły do zniekształcenia zbioru treningowego i destabilizacji procesu destylacji.

---

## 4. Uczestnicy raportu
- Kamil Włodarczyk
- Łukasz Jastrzębski
- Karol Rzepiński