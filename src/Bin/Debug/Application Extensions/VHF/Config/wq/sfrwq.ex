9 40824 8016	1	5 # npolut, numreach, numseg, output_polut_index1,output_polut_index2,
1	1	1	1	1	1	0	0	0	0 #enable_sfrwq_out, enable_sfr_div, enable_sfr_ps, enable_sfrwq_dcxout, enable_nps2sfr, enable_sfrwq_wasp,enable_gwsfr_sacle,enable_interflow_scale,enable_reachotherwq_out,enable_div_wq 
1  1 #The number of npolut and reach id for sfrwq out, only for enable_sfrwq_out=1
.\output\sfr_wq.csv	#sfrwq_out_file
.\Input\wq\sfrwq_div.ex  #sfr river diversion file
.\Input\wq\sfrwq_ps.ex  #sfr point source file
1.0	1.0#sfr2gw_conc_factor, gw2sfr_conc_factor, only for enable_gwsfr_scale=1
1.0   #interflow_conc_scale(1,:) (numseg), only for enable_interflow_scale=1
1.0   #interflow_conc_scale(2,:) (numseg), only for enable_interflow_scale=1
1.0	1500	4 #div_wq_factor,maxso4,maxfu
0	0	0	0	0	0	0	0	0	0	0	#kDecay 1/day  (npolut)
0 # pollution 1 flag  (0 means space constant, 1 means space variable, numreach line) 
0.5	0.1	2.0	0.1 # concentration of pollution NO3 in rain, interflow, baseflow, riverflow	 £¨interflow_conc is used with enable_nps2sfr=0,riverflow_conc is used for initial river water quality setting)
0	# pollution 2 flag
0.1	0.1	0.1	0.1	#	concentration of pollution ON in rain, interflow, baseflow, riverflow
0	# pollution 3 flag
0.008	0.01	0.03	0.01	#	concentration of pollution P in rain, interflow, baseflow, riverflow
0	# pollution 4 flag
0.001	0.001	0.001 0.001	#	concentration of pollution OP in rain, interflow, baseflow, riverflow
0	# pollution 5 flag
0.1	0.1	0.1	0.1	#	concentration of pollution SENDIMENT in rain, interflow, baseflow, riverflow
0	# pollution 6 flag
0.001	0.001	0.001 0.001 #	concentration of pollution chla in rain, interflow, baseflow, riverflow
0	# pollution 7 flag
0.5	0.5	0.5	0.5	#	concentration of pollution CBOD in rain, interflow, baseflow, riverflow
0	# pollution 8 flag
7.0	4.0	4.0	7.0	#	concentration of pollution DO in rain, interflow, baseflow, riverflow
0	# pollution 9 flag
0.05	0.05	0.05	0.05	#	concentration of pollution NH3 in rain, interflow, baseflow, riverflow
0	# pollution 10 flag
50	100	30	100	#	concentration of pollution SO4 in rain, interflow, baseflow, riverflow
0	# pollution 11 flag
0.05	0.1	0.5	0.1	#	concentration of pollution FU in rain, interflow, baseflow, riverflow
