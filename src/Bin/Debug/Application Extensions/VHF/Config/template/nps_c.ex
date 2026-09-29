2	1	1	1	0	0	0	0	0 # num_crop, C_plant_output, Fertilizer_on, sz_month_layer_output,int_temp(i), i =1,9
0	1	0	0  #Init_vars_from_file, Save_vars_to_file, hotstart_GSFLOW_FLAG, hotstart_GSFLOW_WRITE
0.05        #decomposition rate constant litter 1 (1/day) meta (0.03-0.10 day-1)
0.01        #decomposition rate constant litter 2 (1/day) struc (0.001-0.03 day-1)
0.01        #decomposition rate constant SOM 1 (1/day) micr (1 year)
0.0008        #decomposition rate constant SOM 2 (1/day) slow  (6 year)
0.00004        #decomposition rate constant SOM 3 (1/day) passive (200 year)
0.45       #Respiration fractions for litter 1
0.50       #Respiration fractions for litter 2
0.50       #Respiration fractions for SOM 1
0.52       #Respiration fractions for SOM 2
0.55       #Respiration fractions for SOM 3
20.0      #C:N ratio for litter 1 pools
50.0      #C:N ratio for litter 2 pools
10.0      #C:N ratio for SOM 1 pools
20.0      #C:N ratio for SOM 2 pools
40.0      #C:N ratio for SOM 3 pools
1.0       #acceleration term for litter 1
1.0       #acceleration term for litter 2
1.0       #acceleration term for SOM 1
15.0      #acceleration term for SOM 2
675.0     #acceleration term for SOM 3
240.0      #C:P ratio for litter 1 pools  C:N*N:P
750.0      #C:P ratio for litter 2 pools
120.0      #C:P ratio for SOM 1 pools
300.0      #C:P ratio for SOM 2 pools
800.0      #C:P ratio for SOM 3 pools
0.05        #decomposition rate constant DOC 1 (1/day)
0.01        #decomposition rate constant DOC 2 (1/day)
0.005        #decomposition rate constant DOC 3 (1/day)
0.50        #Respiration fractions for DOC 1
0.52        #Respiration fractions for DOC 2
0.55        #Respiration fractions for DOC 3
0.1        #the coefficient of priming effect for root exudates, alpha_RE
2.0        #quilibrium partition coefficient (L water per kilogram of soil) Kd_DOC, Kd increase, concentration decrease.
6.0        #partition coefficient for ammonium, K_NH4 (L/kg)  [0.5 8]
2.0        #K_NO3
8.0        #K_PO4  8.0
0.07       #coef_perco_DOC
0.005       #coef_perco_NH4 Ammonium percolation coefficient (beta1)  [0.01 1] adjust the ammonium concentration in surface runoff
0.015       #coef_perco_NO3 beta_NO3  NPERCO  nitrate percolation coefficient     0.025
0.1       #coef_perco_PO4
2.0       #coef_perco_DIC
1.4       #ratio_erosion_adjust
0.56	  #exp_erosion_runoff, exponent for MUSLE runoff factor calculation, 0.56 is default in SWAT model
0.2       #coef_ratio_enrich_SOC, coefficient of organic carbon enrichment ratio for sediment loading
2.74e-7   #Diffusion coefficient used for bioturbation for litter and soil carbon (m2 d-1)
1.06e-5   #Molecular diffusion coefficient of DOC (m2 d-1)
30.0       #time constant controlling the rate of replenishment of CSxs(currently set to 30 days)
30.0 		#soil pCO2 ratio with atmosphere CO2  (10-50)
0.01    #p_lwt,  annual live wood turnover fraction
0.02    #mor_dw_ann, annual dead wood mortality fraction
1         # LAI_type，  1:calculate according to leaf carbon pool (Noah-MP), 2:calculate according to PHU and LAImax, 3: external input
0.01 0.05 0.01 0.45 0.54   #ratio for initial carbon form, lit_meta, lit_struc, OC_micr, OC_slow, OC_passive for the first soil layer
0.002 0.005 0.005 0.25 0.745   #ratio for initial carbon form, lit_meta, lit_struc, OC_micr, OC_slow, OC_passive for the other soil layer
0.00002 0.00002  #ratio of NH4 in TN, first layer, other layer
0.0004 0.0004  #ratio of NO3 in TN, first layer, other layer
0.005  0.005    #ratio of PO4_solution in TP, first layer, other layer
24   #river_pH_flag  0 is no revision(one data is zero), 1 is constant rivision (one data), 24 is annual rivision for 24 years (24 data)
0.0	0.0	0.0	0.0	0.0	0.0	0.0	0.0	0.0	0.0	0.0	0.0 0.0 0.0 -0.5 0.5 0.5 0.5 0.5 0.5 0.5 0.5 0.5 0.5 0.5 #river water pH revision  0 means no revision
0.001 0.001 0.00015 0.01 0.001 0.00015 0.005  #concentration of NH4,NO3, PO4, DOC, DON, DOP and DIC in solid load (kg /kg sediment)
0.02  0.3  0.015   2.0   0.08   0.0052  7.5 #concentration of NH4,NO3, PO4, DOC, DON, DOP and DIC in groundwater, for gw_upflow (mg/L)
0.01	0.001	0.0001  #rain_conc_OC, rain_conc_ON,rain_conc_OP (mg/L)
0.001	0.0001	0.00001  #OC_drydep, ON_drydep, OP_drydep (kg/ha)
1 5    #idplt value for crop
100 140 18 1900 290 0.03	0.5	0.55	0.2	0.1	0.2	 0.2	5	2	30	60	50	20	400	1000	800 	200  #crop variables
110 160 20 2100 320 0.03	0.5	0.45	0.2	0.1	0.15 0.35	5	2	40	80	70	35	500	1200	1000	250  #crop variables
1  #Hru grid ID for C_plant_output 40551: FLUX station (BET); 34864: crop 11618; 40552 BDT (main type) 26159: grass/crop, 1754 grass
.\output\C_plant_40551.csv   #C_plant_output file
2
.\output\C_plant_38494.csv
3
.\output\C_plant_19952.csv
0.19  0.55  0.26  #ratio of NH4, NO3, ON for nitrogen fertilizer  ratio_ferti_N  0.3
0.5  0.5     #ratio of PO4 solution, minP for phosphorus fertilizer  ratio_ferti_P 0.5
3 5 #num_fert_count num_fert_cell
101 122 161 # fert_time
1 2 3 4 5 
.\input\WQ\ferti_N.txt  #nitrogen fertilizer file (kg N/ha) 
.\input\WQ\ferti_P.txt  #phosphorus fertilizer file  (kg P/ha)
.\input\WQ\ferti_C.txt  #carbon fertilizer file  (kg C/ha)
not used
