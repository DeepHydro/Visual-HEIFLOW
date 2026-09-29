24 17673 2835	1	5 # npolut, numreach, numseg, output_polut_index1,output_polut_index2,
1	0	1	1	1	1	0	0	0	0 #enable_sfrwq_out, enable_sfr_div, enable_sfr_ps, enable_sfrwq_dcxout, enable_nps2sfr, enable_sfrwq_wasp,enable_gwsfr_sacle,enable_interflow_scale,enable_reachotherwq_out,enable_div_wq 
1  1 #The number of npolut and reach id for sfrwq out, only for enable_sfrwq_out=1
.\output\sfrwq_out_reach1.csv	#sfrwq_out_file
.\Input\wq\sfrwq_div.ex  #sfr river diversion file
.\Input\wq\sfrwq_ps.ex  #sfr point source file
1.0	1.0 #sfr2gw_conc_factor, gw2sfr_conc_factor, only for enable_gwsfr_scale=1
1.0   #interflow_conc_scale(1,:) (numseg), only for enable_interflow_scale=1
1.0   #interflow_conc_scale(2,:) (numseg), only for enable_interflow_scale=1
1.0	1500	4 #div_wq_factor,maxso4,maxfu
0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	0	#kDecay 1/day  (npolut)
0	# pollution 1 flag  NH4(0 means space constant, 1 means space variable, numreach line) 
0.01	0.01	0.01	0.01	#	concentration of pollution NH4 in rain, interflow, baseflow, riverflow
0   # pollution 2 flag  NO3(0 means space constant, 1 means space variable, numreach line) 
0.2 	0.2		0.2		0.2 	# concentration of pollution NO3 in rain, interflow, baseflow, riverflow	 £¨interflow_conc is used with enable_nps2sfr=0,riverflow_conc is used for initial river water quality setting)
0	# pollution 3 flag  PO4
0.01	0.01	0.01 	0.01	#	concentration of pollution PO4 in rain, interflow, baseflow, riverflow
0	# pollution 4 flag  DOC1
1.0		0.00	0.20 	0.00	#	concentration of pollution DOC1 in rain, interflow, baseflow, riverflow
0	# pollution 5 flag  DOC2
0.5 	0.00	0.60 	0.00	#	concentration of pollution DOC2 in rain, interflow, baseflow, riverflow
0	# pollution 6 flag  DOC3
0.5 	0.00	1.20	0.00	#	concentration of pollution DOC3 in rain, interflow, baseflow, riverflow
0	# pollution 7 flag  DON1
0.1		0.00	0.02	0.1		#	concentration of pollution DON1 in rain, interflow, baseflow, riverflow
0	# pollution 8 flag  DON2
0.025	0.00	0.03	0.00	#	concentration of pollution DON2 in rain, interflow, baseflow, riverflow
0	# pollution 9 flag  DON3
0.0125	0.00	0.03	0.00	#	concentration of pollution DON3 in rain, interflow, baseflow, riverflow
0	# pollution 10 flag  DOP1
0.0083	0.00  	0.0017 0.001	#	concentration of pollution DOP1 in rain, interflow, baseflow, riverflow
0	# pollution 11 flag  DOP2
0.0017	0.00	0.002	0.00	#	concentration of pollution DOP2 in rain, interflow, baseflow, riverflow
0	# pollution 12 flag  DOP3
0.0006	0.00	0.0015	0.00	#	concentration of pollution DOP3 in rain, interflow, baseflow, riverflow
0	# pollution 13 flag  DIC
5.0 	0.00	7.5		0.00	#	concentration of pollution DIC in rain, interflow, baseflow, riverflow
0	# pollution 14 flag  SOC
0.3 	0.00	0.2		0.00	#	concentration of pollution SOC in rain, interflow, baseflow, riverflow
0	# pollution 15 flag  SON
0.00625	0.00	0.01	0.00	#	concentration of pollution SON in rain, interflow, baseflow, riverflow
0	# pollution 16 flag  SOP
0.00039	0.00	0.00063	0.00	#	concentration of pollution SOP in rain, interflow, baseflow, riverflow
0	# pollution 17 flag  PO4_minP
0.000	0.00	0.00	0.00	#	concentration of pollution PO4_minP in rain, interflow, baseflow, riverflow
0	# pollution 18 flag  PHYT
0.000	0.00	0.00	0.00	#	concentration of pollution PHYT in rain, interflow, baseflow, riverflow
0	# pollution 19 flag  sed
0.0 	0.02	0.015 	0.02	#	concentration of pollution SENDIMENT in rain, interflow, baseflow, riverflow
0	# pollution 20 flag  DO
7.0		4.0		4.0		7.0		#	concentration of pollution DO in rain, interflow, baseflow, riverflow
0	# pollution 21 flag  temp
20.0	20.0	20.0	20.0	#	concentration of pollution TEMP in rain, interflow, baseflow, riverflow
0	# pollution 22 flag  [H+]pH
3.16e-8	3.16e-8	3.16e-8	3.16e-8	#	concentration of pollution pH in rain, interflow, baseflow, riverflow
0	# pollution 23 flag  PHYTN
0.00	0.00	0.00	0.00	#	concentration of pollution PHYTN in rain, interflow, baseflow, riverflow
0	# pollution 24 flag  PHYTP
0.00	0.00	0.00	0.00	#	concentration of pollution PHYTP in rain, interflow, baseflow, riverflow
0	# pollution 10 flag
50		100		30		100		#	concentration of pollution SO4 in rain, interflow, baseflow, riverflow
0	# pollution 11 flag
0.05	0.1		0.5		0.1		#	concentration of pollution FU in rain, interflow, baseflow, riverflow
