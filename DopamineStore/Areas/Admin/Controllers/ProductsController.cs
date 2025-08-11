using DopamineStore.Data;
using DopamineStore.Models;
using DopamineStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DopamineStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ImageService _imageService;

        public ProductsController(ApplicationDbContext context, ImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public async Task<IActionResult> Index(string searchString, int? categoryId)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCategory"] = categoryId;
            var productsQuery = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchString));
            }
            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
            }
            productsQuery = productsQuery.OrderByDescending(p => p.Id);

            ViewBag.Categories = new SelectList(await _context.Categories.OrderBy(c => c.Name).ToListAsync(), "Id", "Name", categoryId);
            return View(await productsQuery.ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            await PopulateCategoriesDropDownList();
            return View(new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, List<IFormFile> imageFiles)
        {
            ModelState.Remove(nameof(product.Category));
            if (await _context.Products.AnyAsync(p => p.Name == product.Name))
            {
                ModelState.AddModelError("Name", "اسم المنتج موجود بالفعل.");
            }

            if (ModelState.IsValid)
            {
                if (imageFiles != null && imageFiles.Count > 0)
                {
                    bool isFirstImage = true;
                    foreach (var file in imageFiles)
                    {
                        string imageUrl = await _imageService.SaveImageAsync(file);
                        if (isFirstImage)
                        {
                            product.ImageUrl = imageUrl;
                            isFirstImage = false;
                        }
                        product.ProductImages.Add(new ProductImg { ImageUrl = imageUrl });
                    }
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "تم إضافة المنتج بنجاح!";
                return RedirectToAction(nameof(Index));
            }

            await PopulateCategoriesDropDownList(product.CategoryId);
            return View(product);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            await PopulateCategoriesDropDownList(product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id) return NotFound();

            ModelState.Remove("Category");

            var productToUpdate = await _context.Products.FindAsync(id);
            if (productToUpdate == null) return NotFound();

            if (ModelState.IsValid)
            {
                productToUpdate.Name = product.Name;
                productToUpdate.Description = product.Description;
                productToUpdate.Price = product.Price;
                productToUpdate.OldPrice = product.OldPrice;
                productToUpdate.OfferEndDate = product.OfferEndDate;
                productToUpdate.StockQuantity = product.StockQuantity;
                productToUpdate.CategoryId = product.CategoryId;
                productToUpdate.IsFeatured = product.IsFeatured;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "تم حفظ تعديلات المنتج بنجاح!";
                return RedirectToAction(nameof(Edit), new { id = product.Id });
            }

            await PopulateCategoriesDropDownList(product.CategoryId);
            product.ProductImages = await _context.ProductImgs.Where(pi => pi.ProductId == id).ToListAsync();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImages(int id, List<IFormFile> files)
        {
            var product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    string imageUrl = await _imageService.SaveImageAsync(file);
                    product.ProductImages.Add(new ProductImg { ImageUrl = imageUrl });

                    if (string.IsNullOrEmpty(product.ImageUrl))
                    {
                        product.ImageUrl = imageUrl;
                    }
                }
                await _context.SaveChangesAsync();
            }
            return PartialView("_ProductImageGallery", product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var image = await _context.ProductImgs.FindAsync(imageId);
            if (image == null) return Json(new { success = false, message = "لم يتم العثور على الصورة" });

            var product = await _context.Products.FindAsync(image.ProductId);

            _imageService.DeleteImage(image.ImageUrl);

            _context.ProductImgs.Remove(image);

            if (product != null && product.ImageUrl == image.ImageUrl)
            {
                var nextImage = await _context.ProductImgs
                    .Where(pi => pi.ProductId == product.Id && pi.Id != imageId)
                    .FirstOrDefaultAsync();
                product.ImageUrl = nextImage?.ImageUrl;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetMainImage([FromBody] SetMainImageViewModel model)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == model.ProductId);

            if (product == null)
                return Json(new { success = false, message = "المنتج غير موجود" });

            var selectedImage = product.ProductImages.FirstOrDefault(img => img.Id == model.ImageId);
            if (selectedImage == null)
                return Json(new { success = false, message = "الصورة غير موجودة" });

            product.ImageUrl = selectedImage.ImageUrl;

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        public class SetMainImageViewModel
        {
            public int ProductId { get; set; }
            public int ImageId { get; set; }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(m => m.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            foreach (var image in product.ProductImages)
            {
                _imageService.DeleteImage(image.ImageUrl);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "تم حذف المنتج بنجاح.";
            return RedirectToAction(nameof(Index));
        }


        private async Task PopulateCategoriesDropDownList(object selectedCategory = null)
        {
            var categoriesQuery = _context.Categories.OrderBy(c => c.Name);
            ViewBag.CategoryId = new SelectList(await categoriesQuery.AsNoTracking().ToListAsync(), "Id", "Name", selectedCategory);
        }
    }
}
