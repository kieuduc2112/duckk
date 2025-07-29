# Script để fix vấn đề admin
Write-Host "Đang chạy ứng dụng..." -ForegroundColor Green
Start-Process -FilePath "dotnet" -ArgumentList "run", "--urls", "http://localhost:5000" -WindowStyle Hidden

Write-Host "Đợi ứng dụng khởi động..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

Write-Host "Đang xóa tất cả users và tạo lại admin..." -ForegroundColor Green
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/DbTest/CleanAll" -UseBasicParsing
    Write-Host "Kết quả:" -ForegroundColor Cyan
    Write-Host $response.Content
} catch {
    Write-Host "Lỗi khi truy cập: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "Đang kiểm tra tình trạng..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/DbTest" -UseBasicParsing
    Write-Host "Tình trạng hiện tại:" -ForegroundColor Cyan
    Write-Host $response.Content
} catch {
    Write-Host "Lỗi khi kiểm tra: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "Hoàn thành!" -ForegroundColor Green 