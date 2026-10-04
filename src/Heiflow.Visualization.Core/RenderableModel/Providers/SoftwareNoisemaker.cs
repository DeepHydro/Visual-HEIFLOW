using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using HUST.WREIS.Dot3D;
using Utility;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Runtime.InteropServices;

namespace HUST.WREIS.Dot3D.Renderable
{
    public class SoftwareNoisemaker
    {
        public SoftwareNoisemaker(int sX, int sY, SeaParameter para, Device dev)
        {
            Initialize();

            device = dev;
            sizeX = sX;
            sizeY = sY;
            prm = para;
            time = 0.0;
            last_time = DateTime.Now.ToBinary();
            octaves = 0;	// don't want to have the noise accessed before it's calculated

            f_sizeX = (float)sizeX;
            f_sizeY = (float)sizeY;

            // reset normals
            vertices = new SOFTWARESURFACEVERTEX[sizeX * sizeY];
            for (int v = 0; v < sizeY; v++)
            {
                for (int u = 0; u < sizeX; u++)
                {
                    vertices[v * sizeX + u].nx = 0.0f;
                    vertices[v * sizeX + u].ny = 1.0f;
                    vertices[v * sizeX + u].nz = 0.0f;
                    vertices[v * sizeX + u].tu = (float)u / (sizeX - 1);
                    vertices[v * sizeX + u].tv = (float)v / (sizeY - 1);
                }
            }
            init_noise();
        }

        private Device device;

        public int n_bits = 5;
        public int n_size;
        public int n_size_m1;
        public int n_size_sq;
        public int n_size_sq_m1;
        public int n_packsize = 4;

        public int np_bits;
        public int np_size;
        public int np_size_m1;
        public int np_size_sq;
        public int np_size_sq_m1;

        public int n_dec_bits = 12;
        public int n_dec_magn = 4096;
        public int n_dec_magn_m1 = 4095;

        public int max_octaves = 32;

        public int noise_frames = 256;
        public int noise_frames_m1;

        public int noise_decimalbits = 15;
        public int noise_magnitude;

        public int scale_decimalbits = 15;
        public int scale_magnitude;

        public int nmapsize_x = 512;
        public int nmapsize_y = 1024;

        public SOFTWARESURFACEVERTEX[] vertices;

        int sizeX, sizeY;	// framebuffer size
        float f_sizeX, f_sizeY;
        float[] framebuffer = null;
        int[] noise;
        int[] o_noise;
        int[] p_noise;
        int[] r_noise;
        int octaves;
        Vector3 e_u, e_v;
        SeaParameter prm;
        int[] multitable;
        long last_time;
        float[] f_multitable;
        double time;
        float RAND_MAX = 0x7fff;
        // remember these
        Vector4 t_corners0, t_corners1, t_corners2, t_corners3;

        Texture[] packed_noise_texture = new Texture[2];
        Texture heightmap, normalmap;

        Surface depthstencil;
  //      Effect hmap_effect, nmap_effect;

        private void Initialize()
        {
            n_size = (1 << (n_bits - 1));
            n_size_m1 = (n_size - 1);
            n_size_sq = (n_size * n_size);
            n_size_sq_m1 = (n_size_sq - 1);
            np_bits = (n_bits + n_packsize - 1);
            np_size = (1 << (np_bits - 1));
            np_size_m1 = (np_size - 1);
            np_size_sq = (np_size * np_size);
            np_size_sq_m1 = (np_size_sq - 1);

            noise_frames_m1 = (noise_frames - 1);
            noise_magnitude = (1 << (noise_decimalbits - 1));
            scale_magnitude = (1 << (scale_decimalbits - 1));

            noise = new int[n_size_sq * noise_frames];
            o_noise = new int[n_size_sq * max_octaves];
            p_noise = new int[np_size_sq * (max_octaves >> (n_packsize - 1))];

            multitable = new int[max_octaves];
            f_multitable = new float[max_octaves];
            if (framebuffer != null)
            {
            }
        }

        public float readtexel_nearest(int u, int v)
        {
            int lu, lv;
            lu = (u >> n_dec_bits) & n_size_m1;
            lv = (v >> n_dec_bits) & n_size_m1;
            return noise[lv * n_size + lu];
        }


        public int readtexel_linear_dual(int u, int v, int o)
        {
            int iu, iup, iv, ivp, fu, fv;
            iu = (u >> n_dec_bits) & np_size_m1;
            iv = ((v >> n_dec_bits) & np_size_m1) * np_size;

            iup = ((u >> n_dec_bits) + 1) & np_size_m1;
            ivp = (((v >> n_dec_bits) + 1) & np_size_m1) * np_size;

            fu = u & n_dec_magn_m1;
            fv = v & n_dec_magn_m1;

            int ut01 = ((n_dec_magn - fu) * r_noise[iv + iu] + fu * r_noise[iv + iup]) >> n_dec_bits;
            int ut23 = ((n_dec_magn - fu) * r_noise[ivp + iu] + fu * r_noise[ivp + iup]) >> n_dec_bits;
            int ut = ((n_dec_magn - fv) * ut01 + fv * ut23) >> n_dec_bits;
            return ut;
        }

        public float get_height_dual(int u, int v)
        {
            int value = 0;
            r_noise = p_noise;	// pointer to the current noise source octave
            int hoct = octaves / n_packsize;
            for (int i = 0; i < hoct; i++)
            {
                value += readtexel_linear_dual(u, v, 0);
                u = u << n_packsize;
                v = v << n_packsize;
                //r_noise += np_size_sq;
            }

            return value * prm.parames[(int)pParameters.p_fStrength].fData / noise_magnitude;
        }

        private void resize(int sX, int sY)
        {
            sizeX = sX;
            sizeY = sY;

            f_sizeX = sizeX;
            f_sizeY = sizeY;

            vertices = new SOFTWARESURFACEVERTEX[sizeX * sizeY];
            for (int v = 0; v < sizeY; v++)
            {
                for (int u = 0; u < sizeX; u++)
                {
                    vertices[v * sizeX + u].nx = 0.0f;
                    vertices[v * sizeX + u].ny = 1.0f;
                    vertices[v * sizeX + u].nz = 0.0f;
                    vertices[v * sizeX + u].tu = (float)u / (sizeX - 1);
                    vertices[v * sizeX + u].tv = (float)v / (sizeY - 1);
                }
            }
        }

        private void init_noise()
        {
            Random rand = new Random();
            // create noise (uniform)
            float[] tempnoise = new float[n_size_sq * noise_frames];
            for (int i = 0; i < (n_size_sq * noise_frames); i++)
            {
                //this.noise[i] = rand()&0x0000FFFF;		
                float temp = (float)rand.Next(0, (int)RAND_MAX) / RAND_MAX;
                tempnoise[i] = 4 * (temp - 0.5f);
            }

            for (int frame = 0; frame < noise_frames; frame++)
            {
                for (int v = 0; v < n_size; v++)
                {
                    for (int u = 0; u < n_size; u++)
                    {
                        /*float temp = 0.25f * (tempnoise[frame*n_size_sq + v*n_size + u] +
                                              tempnoise[frame*n_size_sq + v*n_size + ((u+1)&n_size_m1)] + 
                                              tempnoise[frame*n_size_sq + ((v+1)&n_size_m1)*n_size + u] +
                                              tempnoise[frame*n_size_sq + ((v+1)&n_size_m1)*n_size + ((u+1)&n_size_m1)]);*/
                        int v0 = ((v - 1) & n_size_m1) * n_size,
                            v1 = v * n_size,
                            v2 = ((v + 1) & n_size_m1) * n_size,
                            u0 = ((u - 1) & n_size_m1),
                            u1 = u,
                            u2 = ((u + 1) & n_size_m1),
                            f = frame * n_size_sq;
                        float temp = (1.0f / 14.0f) * (tempnoise[f + v0 + u0] + tempnoise[f + v0 + u1] + tempnoise[f + v0 + u2] +
                                                tempnoise[f + v1 + u0] + 6.0f * tempnoise[f + v1 + u1] + tempnoise[f + v1 + u2] +
                                                tempnoise[f + v2 + u0] + tempnoise[f + v2 + u1] + tempnoise[f + v2 + u2]);

                        noise[frame * n_size_sq + v * n_size + u] = (int)(noise_magnitude * temp);
                    }
                }
            }
        }

        private void calc_noise()
        {
            octaves = Math.Min(prm.parames[(int)pParameters.p_iOctaves].iData, max_octaves);

            // calculate the strength of each octave
            float sum = 0.0f;
            for (int i = 0; i < octaves; i++)
            {
                f_multitable[i] = (float)Math.Pow(prm.get_float(pParameters.p_fFalloff), 1.0f * i);
                sum += f_multitable[i];
            }

            {
                for (int i = 0; i < octaves; i++)
                {
                    f_multitable[i] /= sum;
                }
            }

            {
                for (int i = 0; i < octaves; i++)
                {
                    multitable[i] = (int)(scale_magnitude * f_multitable[i]);
                }
            }


            long this_time = DateTime.Now.ToBinary();
            double itime = this_time - last_time;
            double lp_itime = 0.0;
            last_time = this_time;
            itime *= 0.001 * prm.get_float(pParameters.p_fAnimspeed);
            lp_itime = 0.99 * lp_itime + 0.01 * itime;
            if (!prm.get_bool(pParameters.p_bPaused))
                time += lp_itime;


            double r_timemulti = 1.0;

            for (int o = 0; o < octaves; o++)
            {
                int[] image = new int[3];
                int[] amount = new int[3];
                double dImage = 0, fraction = 0, orig = time * r_timemulti;
                fraction = orig - (dImage = Math.Floor(orig));
                int iImage = (int)dImage;
                amount[0] = (int)(scale_magnitude * f_multitable[o] * (Math.Pow(Math.Sin((fraction + 2) * Math.PI / 3), 2) / 1.5));
                amount[1] = (int)(scale_magnitude * f_multitable[o] * (Math.Pow(Math.Sin((fraction + 1) * Math.PI / 3), 2) / 1.5));
                amount[2] = (int)(scale_magnitude * f_multitable[o] * (Math.Pow(Math.Sin((fraction) * Math.PI / 3), 2) / 1.5));
                image[0] = (iImage) & noise_frames_m1;
                image[1] = (iImage + 1) & noise_frames_m1;
                image[2] = (iImage + 2) & noise_frames_m1;
                {
                    for (int i = 0; i < n_size_sq; i++)
                    {
                        o_noise[i + n_size_sq * o] = (((amount[0] * noise[i + n_size_sq * image[0]]) >> scale_decimalbits) +
                                                        ((amount[1] * noise[i + n_size_sq * image[1]]) >> scale_decimalbits) +
                                                        ((amount[2] * noise[i + n_size_sq * image[2]]) >> scale_decimalbits));
                    }
                }

                r_timemulti *= prm.get_float(pParameters.p_fTimemulti);
            }


            int octavepack = 0;
            for (int o = 0; o < octaves; o += n_packsize)
            {
                for (int v = 0; v < np_size; v++)
                    for (int u = 0; u < np_size; u++)
                    {
                        p_noise[v * np_size + u + octavepack * np_size_sq] = o_noise[(o + 3) * n_size_sq + (v & n_size_m1) * n_size + (u & n_size_m1)];
                        p_noise[v * np_size + u + octavepack * np_size_sq] += mapsample(u, v, 3, o);
                        p_noise[v * np_size + u + octavepack * np_size_sq] += mapsample(u, v, 2, o + 1);
                        p_noise[v * np_size + u + octavepack * np_size_sq] += mapsample(u, v, 1, o + 2);
                    }
                octavepack++;

            }
        }

        int mapsample(int u, int v, int upsamplepower, int octave)
        {
            int magnitude = 1 << upsamplepower;
            int pu = u >> upsamplepower;
            int pv = v >> upsamplepower;
            int fu = u & (magnitude - 1);
            int fv = v & (magnitude - 1);
            int fu_m = magnitude - fu;
            int fv_m = magnitude - fv;

            int o = fu_m * fv_m * o_noise[octave * n_size_sq + ((pv) & n_size_m1) * n_size + ((pu) & n_size_m1)] +
                    fu * fv_m * o_noise[octave * n_size_sq + ((pv) & n_size_m1) * n_size + ((pu + 1) & n_size_m1)] +
                    fu_m * fv * o_noise[octave * n_size_sq + ((pv + 1) & n_size_m1) * n_size + ((pu) & n_size_m1)] +
                    fu * fv * o_noise[octave * n_size_sq + ((pv + 1) & n_size_m1) * n_size + ((pu + 1) & n_size_m1)];

            return o >> (upsamplepower + upsamplepower);
        }

        // check the point of intersection with the plane (0,1,0,0) and return the position in homogenous coordinates 
        Vector4 calc_worldpos(Vector2 uv, Matrix m)
        {
            // this is hacky.. this does take care of the homogenous coordinates in a correct way, 
            // but only when the plane lies at y=0
            Vector4 origin = new Vector4(uv.X, uv.Y, -1, 1);
            Vector4 direction = new Vector4(uv.X, uv.Y, 1, 1);

            origin = Vector4.Transform(origin, m);
            direction = Vector4.Transform(direction, m);

            direction -= origin;

            float l = -origin.Y / direction.Y;	// assumes the plane is y=0

            Vector4 worldPos = origin + direction * l;
            return worldPos;
        }

        bool render_geometry(Matrix m)
        {
            calc_noise();

            float magnitude = n_dec_magn * prm.get_float(pParameters.p_fScale);
            float inv_magnitude_sq = 1.0f / (prm.get_float(pParameters.p_fScale) * prm.get_float(pParameters.p_fScale));

            Matrix m_inv = Matrix.Invert(m);
            e_u = Vector3.TransformNormal(new Vector3(1, 0, 0), m);
            e_v = Vector3.TransformNormal(new Vector3(0, 1, 0), m);
            e_u.Normalize();
            e_v.Normalize();


            t_corners0 = this.calc_worldpos(new Vector2(0.0f, 0.0f), m);
            t_corners1 = this.calc_worldpos(new Vector2(+1.0f, 0.0f), m);
            t_corners2 = this.calc_worldpos(new Vector2(0.0f, +1.0f), m);
            t_corners3 = this.calc_worldpos(new Vector2(+1.0f, +1.0f), m);

            float du = 1.0f / (sizeX - 1), dv = 1.0f / (sizeY - 1), u = 0.0f, v = 0.0f;
            Vector4 result;
            int i = 0;
            for (int iv = 0; iv < sizeY; iv++)
            {
                u = 0.0f;
                for (int iu = 0; iu < sizeX; iu++)
                {

                    //result = (1.0f-v)*( (1.0f-u)*t_corners0 + u*t_corners1 ) + v*( (1.0f-u)*t_corners2 + u*t_corners3 );				
                    result.X = (1.0f - v) * ((1.0f - u) * t_corners0.X + u * t_corners1.X) + v * ((1.0f - u) * t_corners2.X + u * t_corners3.X);
                    result.Z = (1.0f - v) * ((1.0f - u) * t_corners0.Z + u * t_corners1.Z) + v * ((1.0f - u) * t_corners2.Z + u * t_corners3.Z);
                    result.W = (1.0f - v) * ((1.0f - u) * t_corners0.W + u * t_corners1.W) + v * ((1.0f - u) * t_corners2.W + u * t_corners3.W);

                    float divide = 1.0f / result.W;
                    result.X *= divide;
                    result.Z *= divide;

                    vertices[i].x = result.X;
                    vertices[i].z = result.Z;
                    //vertices[i].y = get_height_at(magnitude*result.x, magnitude*result.z, octaves);
                    vertices[i].y = get_height_dual((int)(magnitude * result.X), (int)(magnitude * result.Z));

                    i++;
                    u += du;
                }
                v += dv;
            }

            // smooth the heightdata
            if (prm.parames[(int)pParameters.p_bSmooth].bData)
            {
                //for(int n=0; n<3; n++)
                for (int vv = 1; vv < (sizeY - 1); vv++)
                {
                    for (int uu = 1; uu < (sizeX - 1); uu++)
                    {
                        vertices[vv * sizeX + uu].y = 0.2f * (vertices[vv * sizeX + uu].y +
                            vertices[vv * sizeX + (uu + 1)].y +
                            vertices[vv * sizeX + (uu - 1)].y +
                            vertices[(vv + 1) * sizeX + uu].y +
                            vertices[(vv - 1) * sizeX + uu].y);
                    }
                }
            }

            if (!prm.parames[(int)pParameters.p_bDisplace].bData)
            {
                // reset height to 0
                for (int uu = 0; uu < (sizeX * sizeY); uu++)
                {
                    vertices[uu].y = 0;
                }

            }

            this.upload_noise();

            return true;
        }

        void init_textures()
        {
            // the noise textures. currently two of them (= 8 levels)
            packed_noise_texture[0] = new Texture(device, np_size, np_size, 0, Usage.Dynamic, Format.L16, Pool.Default);
            packed_noise_texture[1] = new Texture(device, np_size, np_size, 0, Usage.Dynamic, Format.L16, Pool.Default);

            heightmap = new Texture(device, nmapsize_x, nmapsize_y, 1, Usage.RenderTarget, Format.A16B16G16R16, Pool.Default);
            normalmap = new Texture(device, nmapsize_x, nmapsize_y, 1, Usage.AutoGenerateMipMap | Usage.RenderTarget, Format.A16B16G16R16, Pool.Default);

            /*device.CreateTexture(nmapsize_x,nmapsize_y,1,D3DUSAGE_RENDERTARGET, D3DFMT_A8R8G8B8, D3DPOOL_DEFAULT, &(this.heightmap),NULL);	
            device.CreateTexture(nmapsize_x,nmapsize_y,1,D3DUSAGE_AUTOGENMIPMAP|D3DUSAGE_RENDERTARGET, D3DFMT_A8R8G8B8, D3DPOOL_DEFAULT, &(this.normalmap),NULL);		*/

            // create z/stencil-buffer
            Bitmap bp = new Bitmap(nmapsize_x, nmapsize_x);
            depthstencil = new Surface(device, bp, Pool.Default);
        }

        void upload_noise()
        {
            System.Drawing.Rectangle locked = new Rectangle();

            int[] data;
            int[] tempdata = new int[np_size_sq];
            for (int t = 0; t < 2; t++)
            {
                int offset = np_size_sq * t;
                // upload the first level
                GraphicsStream gs = packed_noise_texture[t].LockRectangle(0, locked, LockFlags.None);
                data = new int[gs.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = gs.ReadByte();
                }

                for (int i = 0; i < np_size_sq; i++)
                    data[i] = 32768 + p_noise[i + offset];
                packed_noise_texture[t].UnlockRectangle(0);

                int c = packed_noise_texture[t].LevelCount;

                // calculate the second level, and upload it
                gs = packed_noise_texture[t].LockRectangle(1, locked, LockFlags.None);
                data = new int[gs.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = gs.ReadByte();
                }
                int sz = np_size >> 1;
                for (int v = 0; v < sz; v++)
                {
                    for (int u = 0; u < sz; u++)
                    {
                        tempdata[v * np_size + u] = (p_noise[((v << 1)) * np_size + (u << 1) + offset] + p_noise[((v << 1)) * np_size + (u << 1) + 1 + offset] +
                                                   p_noise[((v << 1) + 1) * np_size + (u << 1) + offset] + p_noise[((v << 1) + 1) * np_size + (u << 1) + 1 + offset]) >> 2;
                        data[v * sz + u] = 32768 + tempdata[v * np_size + u];
                    }
                }

                packed_noise_texture[t].UnlockRectangle(1);

                for (int j = 2; j < c; j++)
                {
                    gs = packed_noise_texture[t].LockRectangle(j, locked, LockFlags.None);
                    data = new int[gs.Length];
                    for (int i = 0; i < data.Length; i++)
                    {
                        data[i] = gs.ReadByte();
                    }
                    int pitch = (locked.Height) >> 1;
                    sz = np_size >> j;
                    for (int v = 0; v < sz; v++)
                    {
                        for (int u = 0; u < sz; u++)
                        {
                            tempdata[v * np_size + u] = (tempdata[((v << 1)) * np_size + (u << 1)] + tempdata[((v << 1)) * np_size + (u << 1) + 1] +
                                                        tempdata[((v << 1) + 1) * np_size + (u << 1)] + tempdata[((v << 1) + 1) * np_size + (u << 1) + 1]) >> 2;
                            data[v * pitch + u] = 32768 + tempdata[v * np_size + u];
                        }
                    }
                    packed_noise_texture[t].UnlockRectangle(j);
                }
            }
        }

        void generate_normalmap()
        {
            // do the heightmap thingy
            Surface target, bb;// old_depthstencil;
            bb = device.GetRenderTarget(0);
            target = heightmap.GetSurfaceLevel(0);
            //old_depthstencil=   device.GetRenderTarget(1);	
            
            //hr = device.SetRenderState( D3DRS_CULLMODE, D3DCULL_CCW  );	
            //device.SetStreamSource( 0, surf_software_vertices, 0, sizeof(SOFTWARESURFACEVERTEX) );
            //	 device.set( D3DFVF_SOFTWARESURFACEVERTEX);			
            //device.SetIndices(surf.surf_indicies);
            //hmap_effect.Begin(FX.None);
      //      hmap_effect.BeginPass(0);
      //      hmap_effect.SetValue("scale", prm.parames[(int)pParameters.p_fScale].fData);

            //device.SetTexture(
    //        hmap_effect.SetValue("noise0", packed_noise_texture[0]);
       //     hmap_effect.SetValue("noise1", packed_noise_texture[1]);

            device.SetRenderTarget(0, target);
            //device.SetDepthStencilSurface( depthstencil );
            //device.Clear( 0, NULL,D3DCLEAR_TARGET, D3DCOLOR_XRGB(255,128,28), 1.0f, 0 );
            device.SetRenderState(RenderStates.ZEnable, false);
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, sizeX * sizeY, 0, 2 * (sizeX - 1) * (sizeY - 1));
     //       hmap_effect.EndPass();
     //       hmap_effect.End();

            // calculate normalmap

            target = normalmap.GetSurfaceLevel(0);
            device.SetRenderTarget(0, target);
            //nmap_effect.Begin(FX.None);
            //nmap_effect.BeginPass(0);
            //nmap_effect.SetValue("inv_mapsize_x", 1.0f / nmapsize_x);
            //nmap_effect.SetValue("inv_mapsize_y", 1.0f / nmapsize_y);
            //nmap_effect.SetValue("corner00", t_corners0);
            //nmap_effect.SetValue("corner01", t_corners1);
            //nmap_effect.SetValue("corner10", t_corners2);
            //nmap_effect.SetValue("corner11", t_corners3);
            //nmap_effect.SetValue("amplitude", 2 * prm.parames[(int)pParameters.p_fStrength].fData);
            //nmap_effect.SetValue("hmap", heightmap);
            //device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, sizeX * sizeY, 0, 2 * (sizeX - 1) * (sizeY - 1));
            //nmap_effect.EndPass();
            //nmap_effect.End();

            // restore the device
            device.SetRenderState(RenderStates.ZEnable, true);
            device.SetRenderTarget(0, bb);
         //   device.SetRenderTarget(1,old_depthstencil);

        }
    }
}
