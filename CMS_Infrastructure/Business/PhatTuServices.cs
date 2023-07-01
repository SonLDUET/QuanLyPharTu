using CMS_Core.Enums;
using CMS_Infrastructure.Context;
using CMS_WebDesignCore.Entities;
using CMS_WebDesignCore.Enums;
using dj_webdesigncore.AuthModel;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CMS_Infrastructure.Business
{
    public interface IPhatTuServices
    {
        public Task<List<dynamic>> filter(string phapDanh, int DaHoanTuc,
            GioiTinh GioiTinh, int pageSize, int pageNumber);
        public Task<PhatTu> LayThongTinTheoEmail(string Email);
        public Task<PhatTuState> ThemPhatTu(PhatTu phatTuMoi);
        public Task<PhatTuState> SuaPhatTu(PhatTu phatTuMoi);
        public Task<PhatTuState> XoaPhatTu(int PhatTuId);
        public Task<IEnumerable<DaoTrang>> LocDaoTrang(int KetThuc, int pageSize, int poageNumber);
        public Task<bool> DangKyThamGiaDaoTrang(string UserEmail, int DaoTrangId);
        public Task<bool> XuLyDon(string nguoiXuLyEmail, int DonId, TrangThaiDon state);
        public Task<bool> DiemDanh(int PhatTuDaoTrangId, bool isAttended, string LyDo);
        public Task<bool> MoDaoTrang(DaoTrang daoTrangMoi);
        public Task<bool> KetThucDaoTrang(int DaoTrangId);
        public Task<Response<DaoTrang>> SuaDaoTrang(DaoTrang newDaoTrang);
        public Task<bool> SuaKieuThanhVienChoPhatTu(string Email, string kieu);
        public Task<List<DonDangKy>> LocDonDangKy(string? Ten, string? Email, TrangThaiDon? trangThaiDon, int pageSize, int pageNumber);
    }
    public class PhatTuServices : IPhatTuServices
    {
        private readonly AppDbContext db;

        public PhatTuServices(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<List<dynamic>> filter(string phapDanh, int DaHoanTuc,
            GioiTinh GioiTinh, int pageSize, int pageNumber)
        {
            var res = db.PhatTu.Include(x => x.listPhatTuDaoTrang).AsQueryable();
            if (phapDanh != null)
            {
                res = res.Where(h => h.PhapDanh.StartsWith(phapDanh) || h.PhapDanh == null).AsQueryable();
            }
            if (DaHoanTuc != 0)
            {
                bool isHoanTuc = DaHoanTuc == 1 ? true : false;
                res = res.Where(h => h.DaHoanTuc == isHoanTuc).AsQueryable();
            }
            if (GioiTinh == GioiTinh.NAM && GioiTinh == GioiTinh.NU)
            {
                res = res.Where(h => h.GioiTinh == GioiTinh).AsQueryable();
            }

            List<dynamic> listPhatTu = new List<dynamic>();
            res.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList().ForEach(x =>
            {
                int soBuoiThamGia = x.listPhatTuDaoTrang.Where(x => x.DaThamGia == true).Count();
                listPhatTu.Add(new { HoTen = x.Ho + " " + x.Ten, EmailAddress = x.Email, SoBuoiDaoTrang = soBuoiThamGia });
            });
            return listPhatTu;
        }

        public async Task<PhatTu> LayThongTinTheoEmail(string Email)
        {
            PhatTu phatTu = await db.PhatTu.FirstOrDefaultAsync(x => x.Email == Email);
            return phatTu;
        }

        public async Task<PhatTuState> ThemPhatTu(PhatTu phatTuMoi)
        {
            using (var trans = await db.Database.BeginTransactionAsync())
            {
                try
                {
                    if (db.PhatTu.Any(x => x.Email == phatTuMoi.Email)) return PhatTuState.EMAIL_EXISTED;
                    //if (db.PhatTu.Any(x => x.SoDienThoai == phatTuMoi.SoDienThoai)) return PhatTuState.PHONE_NUMBER_EXISTED;
                    await db.AddAsync(phatTuMoi);
                    await db.SaveChangesAsync();
                    trans.Commit();
                    return PhatTuState.SUCCESS;
                }
                catch (Exception ex)
                {
                    await trans.RollbackAsync();
                }
            }
            return PhatTuState.FAILED;
        }

        public async Task<PhatTuState> SuaPhatTu(PhatTu phatTuMoi)
        {
            using (var trans = await db.Database.BeginTransactionAsync())
            {
                try
                {
                    if (db.PhatTu.Any(x => x.Email == phatTuMoi.Email)) return PhatTuState.EMAIL_EXISTED;
                    if (db.PhatTu.Any(x => x.SoDienThoai == phatTuMoi.SoDienThoai)) return PhatTuState.PHONE_NUMBER_EXISTED;
                    PhatTu phatTuCanSua = db.PhatTu.Find(phatTuMoi.Id);
                    phatTuCanSua.Ho = phatTuMoi.Ho;
                    phatTuCanSua.Ten = phatTuMoi.Ten;
                    phatTuCanSua.AnhChup = phatTuMoi.AnhChup;
                    phatTuCanSua.NgaySinh = phatTuMoi.NgaySinh;
                    phatTuCanSua.NgayHoanTuc = phatTuMoi.NgayHoanTuc;
                    phatTuCanSua.DaHoanTuc = phatTuMoi.DaHoanTuc;
                    phatTuCanSua.SoDienThoai = phatTuMoi.SoDienThoai;
                    phatTuCanSua.NgayXuatGia = phatTuMoi.NgayXuatGia;
                    phatTuCanSua.NgayCapNhat = DateTime.Now.Date;
                   // phatTuCanSua.KieuThanhVienId = phatTuMoi.KieuThanhVienId;
                    phatTuCanSua.GioiTinh = phatTuMoi.GioiTinh;
                    db.PhatTu.Update(phatTuCanSua);
                    await db.SaveChangesAsync();
                    await trans.CommitAsync();
                    return PhatTuState.SUCCESS;
                }
                catch (Exception ex)
                {
                    await trans.RollbackAsync();
                }
                return PhatTuState.FAILED;
            }
        }

        public async Task<bool> SuaKieuThanhVienChoPhatTu(string Email, string kieu)
        {
            using (var trans = await db.Database.BeginTransactionAsync())
            {
                try
                {
                    PhatTu phatTu = await db.PhatTu.SingleOrDefaultAsync(x => x.Email == Email);
                    if (phatTu == null)
                    {
                        return false;
                    }
                    var kieuThanhVien = db.KieuThanhVien.First(x => x.TenKieu == kieu);
                    phatTu.KieuThanhVienId = kieuThanhVien.Id;
                    db.Update(kieuThanhVien);
                    await db.SaveChangesAsync();
                    await trans.CommitAsync();  
                    return true;
                }
                catch (Exception ex)
                {
                    await trans.RollbackAsync();
                }
                return false;
            }
        }

        public async Task<PhatTuState> XoaPhatTu(int PhatTuId)
        {
            PhatTu phatTuCanXoa = await db.PhatTu.FindAsync(PhatTuId);
            if (phatTuCanXoa != null)
            {
                db.PhatTu.Remove(phatTuCanXoa);
                await db.SaveChangesAsync();
                return PhatTuState.SUCCESS;
            }
            return PhatTuState.FAILED;
        }

        public async Task<IEnumerable<DaoTrang>> LocDaoTrang(int KetThuc, int pageSize, int pagerNumber)
        {
            if (KetThuc == 1)  // Da Ket Thuc
            {
                return db.DaoTrang.AsQueryable().Where(x => x.DaKetThuc == true).Skip(pageSize * (pagerNumber - 1)).Take(pageSize).ToList();
            } 
            if (KetThuc == 0) // Chua Ket Thuc
            {
                return db.DaoTrang.AsQueryable().Where(x => x.DaKetThuc == false).Skip(pageSize * (pagerNumber - 1)).Take(pageSize).ToList();
            }
            // Khong loc
            return db.DaoTrang.AsQueryable().Skip(pageSize * (pagerNumber - 1)).Take(pageSize).ToList();

        }

        public async Task<bool> XuLyDon(string nguoiXuLyEmail, int DonId, TrangThaiDon state)
        {
            DonDangKy donXuLy = db.DonDangKy.Find(DonId);
            if (donXuLy == null)
            {
                return false;
            }
            PhatTu nguoiXuLy = await db.PhatTu.FirstOrDefaultAsync(x => x.Email == nguoiXuLyEmail);
            if (nguoiXuLy == null)
            {
                return false;
            }
            donXuLy.NguoiXuLyId = nguoiXuLy.Id;
            donXuLy.TrangThaiDon = state;
            if (state == TrangThaiDon.DONGY)
            {
                PhatTuDaoTrang thamGia = new PhatTuDaoTrang
                {
                    DaoTrangId = donXuLy.DaoTrangId,
                    PhatTuId =  donXuLy.PhatTuId,
                    DaThamGia = false

                };
                await db.PhatTuDaoTrang.AddAsync(thamGia);

                DaoTrang daoTrangDangKy = db.DaoTrang.Find(donXuLy.DaoTrangId);
                daoTrangDangKy.SoThanhVienThamGia++;
                db.Update(daoTrangDangKy);
                await db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DangKyThamGiaDaoTrang(string UserEmail, int DaoTrangId)
        {
            PhatTu phatTu = db.PhatTu.Where(x => x.Email == UserEmail).FirstOrDefault();
            DaoTrang daoTrangDangKy = await db.DaoTrang.FindAsync(DaoTrangId);
            if (phatTu == null || daoTrangDangKy == null)
            {
                return false;
            }
            if (DateTime.UtcNow > daoTrangDangKy.ThoiGianToChuc) return false;
            DonDangKy form = new DonDangKy
            {
                PhatTuId = phatTu.Id,
                DaoTrangId = DaoTrangId,
                NgayGuiDon = DateTime.UtcNow,
                TrangThaiDon = TrangThaiDon.DANGCHO
            };
            await db.AddAsync(form);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DiemDanh(int PhatTuDaoTrangId, bool isAttended, string? LyDo)
        {
            var ptdt = await db.PhatTuDaoTrang.FindAsync(PhatTuDaoTrangId);
            if (ptdt == null)
            {
                return false;
            }
            ptdt.DaThamGia = isAttended;
            ptdt.LyDoKhongThamGia = LyDo ?? "";
            db.Update(ptdt);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MoDaoTrang(DaoTrang daoTrangMoi)
        {
            daoTrangMoi.DaKetThuc = false;
            daoTrangMoi.SoThanhVienThamGia = 0;
            await db.AddAsync(daoTrangMoi);
            int NumOfrowAffected = await db.SaveChangesAsync();
            return NumOfrowAffected != 0;
        }

        public async Task<bool> KetThucDaoTrang(int DaoTrangId)
        {
            DaoTrang daotrangKetThuc = await db.DaoTrang.FindAsync(DaoTrangId);
            if (daotrangKetThuc == null) { return false; }
            daotrangKetThuc.DaKetThuc = true;
            return true;
        }

        public async Task<Response<DaoTrang>> SuaDaoTrang(DaoTrang newDaoTrang)
        {
            using (var trans = await db.Database.BeginTransactionAsync())
            {
                try
                {
                    DaoTrang daoTrang = db.DaoTrang.Find(newDaoTrang.Id);
                    if (daoTrang == null)
                    {
                        return new Response<DaoTrang>
                        {
                            Message = "Đạo tràng không tồn tại",
                            Success = (int)AuthStatusEnum.FAILED
                        };
                    }
                    daoTrang.ThoiGianToChuc = newDaoTrang.ThoiGianToChuc;
                    daoTrang.NguoiChuTriId = newDaoTrang.NguoiChuTriId;
                    daoTrang.NoiDung = newDaoTrang.NoiDung;
                    db.Update(daoTrang);
                    await db.SaveChangesAsync();
                    await trans.CommitAsync();
                    return new Response<DaoTrang>
                    {
                        Message = "Sửa đạo tràng thành công",
                        Success = (int)AuthStatusEnum.SUCCESS,
                        Data = newDaoTrang
                    };
                }
                catch (Exception ex)
                {
                    await trans.RollbackAsync();
                }
            }
            return new Response<DaoTrang>
            {
                Message = "Sửa đạo tràng không thành công",
                Success = (int)AuthStatusEnum.FAILED
            };

        }

        public async Task<List<DonDangKy>> LocDonDangKy(string? Ten, string? Email, TrangThaiDon? trangThaiDon, int pageSize, int pageNumber)
        {
            var res = db.DonDangKy.Include(x => x.PhatTu).AsQueryable();
            if (Ten != null)
            {
                res = res.Where(x => x.PhatTu.Ten.Contains(Ten)).AsQueryable();
            }
            if (Email != null)
            {
                res = res.Where(x => x.PhatTu.Email.Contains(Email)).AsQueryable();
            }
            if (trangThaiDon != null)
            {
                res = res.Where(x => x.TrangThaiDon == trangThaiDon).AsQueryable();
            }
            return await res.Skip(pageSize*(pageNumber - 1)).Take(pageSize).ToListAsync();
        }
    }
}
