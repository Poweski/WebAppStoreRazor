# Web App Store

### EN
The simple online store application was created in **ASP.NET Core Razor Pages** using **Entity Framework** and **Identity**. It allows you to manage categories and products in a one-to-many relationship, where each product has a name, price, image and an assigned category. Administrators have access to the category and product management panel, while regular users can browse the assortment and add products to the cart.
The shopping cart is stored in **cookies** and works independently for each user. Users can view, edit and delete the contents of the cart, and its status is dynamically updated. Placing an order requires logging in and includes a purchase summary, an address form and a choice of payment method.
Additionally, the application offers a **REST API** for managing products and categories, which has been tested in **Postman**. The store interface supports lazy loading of products - subsequent items on the main page are loaded asynchronously using **AJAX**.


### PL
Aplikacja prostego sklepu internetowego została stworzona w **ASP.NET Core Razor Pages** z wykorzystaniem **Entity Framework** i **Identity**. Umożliwia zarządzanie kategoriami i produktami w relacji jeden-do-wielu, gdzie każdy produkt posiada nazwę, cenę, obraz oraz przypisaną kategorię. Administratorzy mają dostęp do panelu zarządzania kategoriami i produktami, natomiast zwykli użytkownicy mogą przeglądać asortyment i dodawać produkty do koszyka. 
Koszyk zakupowy przechowywany jest w **ciasteczkach** i działa niezależnie dla każdego użytkownika. Użytkownicy mogą przeglądać, edytować i usuwać zawartość koszyka, a jego stan jest dynamicznie aktualizowany. Złożenie zamówienia wymaga zalogowania się i obejmuje podsumowanie zakupów, formularz adresowy oraz wybór metody płatności.
Dodatkowo aplikacja oferuje **REST API** do zarządzania produktami i kategoriami, które zostało przetestowane w **Postmanie**. Interfejs sklepu wspiera lazy loading produktów – kolejne pozycje na stronie głównej ładują się asynchronicznie za pomocą **AJAX**.
