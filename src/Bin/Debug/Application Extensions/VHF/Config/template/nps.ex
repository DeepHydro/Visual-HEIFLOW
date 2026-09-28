0	1	0	0	1	0	1	0	0 # num_otherwq, Fertilizer_on, init_nps_extern, init_otherwq_extern, init_gw_extern,init_gw_otherwq_extern, GEHM_NP_flag, int_temp(i), i =1,2
20.0     #average annual air temperature  temp_air  20.0
1.4      #bulk_density bulk density of the soil layer   Mg/m3  1.3-1.6
0.03     #beta_rsd  RSDCO/RSDCO_PL coef_mine_resi  Rate coefficient for mineralization of the residue fresh organic nutrients  0.05
0.00012   #beta_min  CMN  coef_mine_active for ON  Rate coefficient for mineralization of the humus active orgainc nitrogen  0.0003
0.0002   #beta_min2  CMN2  coef_mine_active2 for OP  Rate coefficient for mineralization of the humus active orgainc phosphorus  0.0003  
0.15     #volatilization cation exchange capacity factor  beta_cec,ly  factor_cation  0.15
1.4      #beta_denit  CDN  coef_denitri  Rate coefficient for denitrification  1.4
0.55      #gama_sw,thr  SDNCO   factor_water_threshold  Threshold value of nutrient cycling water factor for denitrification to occur   0.6
0.2      #pai  PSP  Phosphorus availbility index  index_P  0.4 
0.0006   #d-1  slow equilibration rate constant d-1  cons_rate_equi  0.0006 
0.5      #nitrogen fertilizer absorb coefficient  coef_absorb_N  0.5  only used when GEHM_NP_flag = 0, which means a direct constant plant uptake proportion.
0.5      #phosphorus fertilizer absorb coefficient  coef_absorb_P 0.5  only used when GEHM_NP_flag = 0, which means a direct constant plant uptake proportion.
0.2      #ratio of NO3 for nitrogen fertilizer  ratio_ferti_NO3  0.2
0.2      #ratio of NH4 for nitrogen fertilizer  ratio_ferti_NH4  0.3
0.4      #ratio of PO4 for phosphorus fertilizer   ratio_ferti_PO4 0.5
0.3      #fraction of daily rain falling in the half-hour highest intensity rainfall  alpha_half  0.3
0.15     #fraction of porosity from which anions are excluded  ANION_EXCL  0.15
0.025      #beta_NO3  NPERCO  nitrate percolation coefficient   coef_perco_NO3  0.1 
200.0    #k_d,surf  PHOSKD  Phosphorus soil partitioning coefficient(m3/Mg)  coef_parti_PO4 175.0
0.78     #ERORGN coefficient   coefficient of organic nitrogen enrichment ratio  0.78  
0.50     #ERORGP coefficient   coefficient of phosphorus enrichment ratio   0.78
0.78     #ERORGC coefficient   coefficient of organic carbon enrichment ratio, for CBOD calculation   0.78
1000.0   #DIRTMX  SED_mx  maximum accumulation of solids possible for the impervious surface(kg/curb km)   Solid_max_acc 1000.0
5.0      #THALF  the length of time needed for solid build up to increase from 0 to 1/2 SED_mx(days)   Solid_day_half   5.0
0.18     #URBCOEF  wash off coefficient 0.039-0.390 (mm-1)  coef_wash  0.18 
2.0      #CURBDEN  curb length density in impervious surface (curb km/ha)    curb_len_den  2.0 
0.001    #concentration of nitrate in solid load(kg N/kg sediment)   conc_NO3_imper  0.001
0.001    #concentration of organic nitrogen in solid load(kg N/kg sediment)   conc_ON_imper  kg N/kg sediment  0.001
0.00008  #concentration of phosphate in solid load(kg P/kg sediment)   conc_PO4_imper  kg P/kg sediment   0.00008
0.00008  #concentration of organic phosphorus in solid load(kg P/kg sediment)   conc_OP_minP_imper  kg P/kg sediment   0.00008
0.5	0.1 0.003 0.006 # rain_conc_NO3 (mg/L), rain_conc_NH4(mg/L), NO3_drydep(kg N/ha), NH4_drydep(kg N/ha)
0.008  0.0001  #rain_conc_PO4 (mg/L), PO4_drydep(kg P/ha)
0.0	0.0    #kDeay constant  rate constant for degration(1/day)  Rate_degra_wq(num_otherwq) 
50.0  0.05   #mg/L concentration in rainfall for otherwq   rain_conc_wq(num_otherwq)
0.01  0.0001 #kg/ha concentration for dry deposition      wq_drydep(num_otherwq)
0.1 0.1      #wq percolation coefficient beta_wq (num_otherwq)      coef_perco_wq(num_otherwq)
0.005 0.0005 #concentration of wq in solid load(kg wq/kg sediment)  wq_conc_imper(num_otherwq)
3 5
120 150 180
1 2 3 4 5
.\input\WQ\ferti_N.txt  #nitrogen fertilizer file (kg N/ha) 
.\input\WQ\ferti_P.txt  #phosphorus fertilizer file  (kg P/ha)
0  #init_nps_conc_flag  0 means homogeneous(1 line) and 1 means space-varient(nhru line)
.\input\WQ\init_nps_extern.txt  #init_nps_extern file
0  #init_otherwq_conc_flag  0 means homogeneous(1 line) and 1 means space-varient(nhru line)
.\input\WQ\init_otherwq_extern.txt  #init_otherwq_extern file
2 #init_gw_conc_flag  0 means homogeneous(1 line) and 1 means space-varient(nhru line), 2 means update with dynamic parameter
.\input\WQ\init_gw_extern.txt  #init_gw_extern file
0 #init_gw_otherwq_conc_flag 0 means homogeneous(1 line) and 1 means space-varient(nhru line)
.\input\WQ\init_gw_otherwq_extern.txt  #init_gw_otherwq_extern file

