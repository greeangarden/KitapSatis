$ErrorActionPreference = "Stop"
$basUrl = "http://localhost:5229/api"

try {
    Write-Host "1. Admin Login..."
    $adminLogin = Invoke-RestMethod -Method Post -Uri "$basUrl/auth/login" -ContentType "application/json" -Body '{"username":"admin@deu.edu.tr","password":"1234"}'
    $adminToken = $adminLogin.token
    $adminHeaders = @{ Authorization = "Bearer $adminToken"; "Content-Type" = "application/json" }

    Write-Host "2. Create Category..."
    $categoryBody = @{ name = "Yazılım Test Kategori " + (Get-Date).Ticks } | ConvertTo-Json
    $category = Invoke-RestMethod -Method Post -Uri "$basUrl/categories" -Headers $adminHeaders -Body $categoryBody

    Write-Host "3. Create Book..."
    $catId = $category.id
    $bookBody = @"
{
    "name": "Algoritmalara Giris Test",
    "author": "Cormen",
    "price": 250.00,
    "stockQuantity": 50,
    "minStockLevel": 5,
    "categoryId": $catId,
    "imageUrl": "https://example.com/book.jpg",
    "description": "Test book description",
    "isActive": true
}
"@
    $book = Invoke-RestMethod -Method Post -Uri "$basUrl/books" -Headers $adminHeaders -ContentType "application/json" -Body $bookBody

    Write-Host "4. Register Student..."
    $userEmail = "testuser" + (Get-Date).Ticks + "@ogr.deu.edu.tr"
    $regBody = @{
        firstName  = "Test"
        lastName   = "User"
        email      = $userEmail
        password   = "password123"
        department = "Bilgisayar Muhendisligi"
    } | ConvertTo-Json
    $null = Invoke-RestMethod -Method Post -Uri "$basUrl/auth/register" -ContentType "application/json" -Body $regBody

    Write-Host "5. Student Login..."
    $studentLogin = Invoke-RestMethod -Method Post -Uri "$basUrl/auth/login" -ContentType "application/json" -Body "{""username"":""$userEmail"",""password"":""password123""}"
    $studentToken = $studentLogin.token
    $studentHeaders = @{ Authorization = "Bearer $studentToken"; "Content-Type" = "application/json" }

    Write-Host "6. Add to Cart..."
    $cartBody = @{ bookId = $book.id; quantity = 2 } | ConvertTo-Json
    $null = Invoke-RestMethod -Method Post -Uri "$basUrl/cart/items" -Headers $studentHeaders -Body $cartBody

    Write-Host "7. Checkout..."
    $checkoutBody = @{ deliveryAddress = "Buca/İzmir Test" } | ConvertTo-Json
    $order = Invoke-RestMethod -Method Post -Uri "$basUrl/cart/checkout" -Headers $studentHeaders -Body $checkoutBody

    Write-Host "8. Check Dashboard..."
    $dashboard = Invoke-RestMethod -Method Get -Uri "$basUrl/dashboard/summary" -Headers $adminHeaders
    
    Write-Host "`n--- TEST BAŞARILI ---"
    Write-Host "Sipariş No: $($order.orderNumber)"
    Write-Host "Toplam Tutar: $($order.totalPrice)"
    Write-Host "Dashboard Toplam Sipariş: $($dashboard.totalOrders)"
    Write-Host "Dashboard Toplam Gelir: $($dashboard.totalRevenue)"
    Write-Host "--------------------"

}
catch {
    Write-Host "HATA OLUŞTU: $_"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $reader.BaseStream.Position = 0
        $reader.DiscardBufferedData()
        $responseBody = $reader.ReadToEnd()
        $responseBody | Out-File "error.txt"
    }
    exit 1
}
