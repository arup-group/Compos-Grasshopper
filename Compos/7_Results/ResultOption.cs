namespace ComposAPI
{
  public enum ResultOption
  {
    CRITI_SECT_DIST, // Current position/distance from left end of the member
    CRITI_SECT_ATTRI, // Position attributes (max shear, moment and etc.,)
    ULTI_MOM_CONS, // Construction stage ultimate moment
    ULTI_MOM_FINA, // Final stage ultimate moment
    ULTI_SHE_CONS, // Construction stage ultimate shear
    ULTI_SHE_FINA, // Final stage ultimate shear
    ULTI_AXIAL_CONS, // Construction stage ultimate axial force
    ULTI_AXIAL_FINA, // Final stage ultimate axial force
    WORK_MOM_CONS_DEAD, // Construction stage working dead load moment
    WORK_MOM_CONS_LIVE, // Construction stage working live load moment
    WORK_MOM_FINA_ADDI, // Final stage working additional dead load moment
    WORK_MOM_FINA_LIVE, // Final stage working live load moment
    WORK_MOM_FINA_SHRI, // Final stage working shrinkage moment
    WORK_SHE_CONS_DEAD, // Construction stage working dead load shear
    WORK_SHE_CONS_LIVE, // Construction stage working live load shear
    WORK_SHE_FINA_ADDI, // Final stage working additional dead load shear
    WORK_SHE_FINA_LIVE, // Final stage working live load shear
    WORK_AXIAL_CONS_DEAD, // Construction stage working dead load axial
    WORK_AXIAL_CONS_LIVE, // Construction stage working live load axial
    WORK_AXIAL_FINA_ADDI, // Final stage working additional dead load axial
    WORK_AXIAL_FINA_LIVE, // Final stage working live load axial
    CAPA_MOM_ULTI_CONS_HOG, // Hogging moment capacity in Construction stage
    NEUTRAL_X_ULTI_CONS_HOG, // Neutral axis depth under Hogging moment in Construction stage
    CAPA_MOM_ULTI_FINA_HOG, // Hogging moment capacity in Final stage
    NEUTRAL_X_ULTI_FINA_HOG, // Neutral axis depth under Hogging moment in Final stage
    CAPA_MOM_ULTI_CONS_SAG, // Sagging moment capacity in Construction stage
    NEUTRAL_X_ULTI_CONS_SAG, // Neutral axis depth under Sagging moment in Construction stage
    CAPA_MOM_ULTI_FINA_SAG, // Sagging moment capacity in Final stage
    NEUTRAL_X_ULTI_FINA_SAG, // Neutral axis depth under Sagging moment in Final stage
    CAPA_SHE_SHEAR, // shear capacity
    CAPA_SHE_BUCLE, // shear capacity with web buckling
    CAPA_SHE_PV, // Used shear capacity
    CAPA_MOM_BEAM_PLAS_HOG, // Assumed plastic Hogging moment capacity in Construction stage
    NEUTRAL_X_BEAM_PLAS_HOG, // Neutral axis depth under Assumed plastic Hogging moment in Construction stage
    CAPA_MOM_100_INTER_HOG, // Assumed 100% shear interaction hogging moment capacity in final stage
    NEUTRAL_X_100_INTER_HOG, // Neutral axis depth under assumed 100% interaction hogging moment in final stage
    CAPA_MOM_BEAM_PLAS_SAG, // Assumed plastic Sagging moment capacity in Construction stage
    NEUTRAL_X_BEAM_PLAS_SAG, // Neutral axis depth under Assumed plastic Sagging moment in Construction stage
    CAPA_MOM_100_INTER_SAG, // Assumed 100% shear interaction sagging moment capacity in final stage
    NEUTRAL_X_100_INTER_SAG, // Neutral axis depth under assumed 100% interaction sagging moment in final stage
    SLAB_WIDTH_L_EFFECT, // Effective slab width on left side
    SLAB_WIDTH_R_EFFECT, // Effective slab width on right side
    GIRDER_WELD_THICK_T, // Welding thickness at top
    GIRDER_WELD_THICK_B, // Welding thickness at bottom
    I_STEEL_BEAM, // moment of Inertia of steel beam
    X_STEEL_BEAM, // Neutral axis depth of steel beam
    AREA_STEEL_BEAM, // Area of steel beam
    I_LONG_TERM, // moment of Inertia of beam for long term loading
    X_LONG_TERM, // Neutral axis depth of beam for long term loading
    AREA_LONG_TERM, // Area of beam for long term loading
    I_SHORT_TERM, // moment of Inertia of beam for short term loading
    X_SHORT_TERM, // Neutral axis depth of beam for short term loading
    AREA_SHORT_TERM, // Area of beam for short term loading
    I_SHRINK, // moment of Inertia of beam for shrinkage
    X_SHRINK, // Neutral axis depth of beam for shrinkage
    AREA_SHRINK, // Area of beam for shrinkage
    I_EFFECTIVE, // Effective moment of Inertia of beam
    X_EFFECTIVE, // Neutral axis depth of beam under combined loading
    AREA_EFFECT, // Effective Area of beam
    I_VIBRATION, // moment of Inertia of beam for vibration
    X_VIBRATION, // Neutral axis depth of beam for vibration
    AREA_VIBRATION, // Area of beam for vibration
    DEFL_CONS_DEAD_LOAD, // Deflection due to Construction dead loads
    DEFL_ADDI_DEAD_LOAD, // Deflection due to additional dead loads
    DEFL_FINA_LIVE_LOAD, // Deflection due to Final stage live loads
    DEFL_SHRINK, // Deflection due to shrinkage of concrete
    DEFL_POST_CONS, // Deflection due to post Construction loads
    DEFL_FINA_TOTAL, // Total Deflection
    MODAL_SHAPE, // Mode shape
    STUD_CONCRTE_FORCE, // Actual stud capacity
    STUD_NUM_LEFT_PROV, // Actual number of studs provided from left end
    STUD_NUM_RIGHT_PROV, // Actual number of studs provided from right end
    STUD_NUM_LEFT_USED, // Used number of studs provided from left end
    STUD_NUM_RIGHT_USED, // Used number of studs provided from right end
    STUD_CONCRTE_FORCE_100, // Required stud capacity for 100% shear interaction
    STUD_CONCRTE_FORCE_REQ, // Required stud capacity for given moment
    STUD_INTERACT_REQ, // Required shear interaction for given moment
    STUD_ONE_CAPACITY, // One shear stud capacity
    STUD_PERCENT_INTERACTION, // Actual shear interaction
    STUD_CAPACITY_LEFT, // Actual shear capacity from left end
    STUD_CAPACITY_RIGHT, // Actual shear capacity from right end
    CLAS_CONS_FLAN_CLASS, // Flange class in Construction stage
    CLAS_CONS_WEB_CLASS, // web class in Construction stage
    CLAS_CONS_SECTION, // Section class in Construction stage
    CLAS_FINA_FLAN_CLASS, // Flange class in Final stage
    CLAS_FINA_WEB_CLASS, // web class in Final stage
    CLAS_FINA_SECTION, // Section class in Final stage
    STRESS_CONS_BEAM_BOT, // Maximum stress in steel beam bottom Flange due to Construction loads
    STRESS_CONS_BEAM_WEB, // Maximum stress in steel beam web due to Construction loads
    STRESS_CONS_BEAM_TOP, // Maximum stress in steel beam top Flange due to Construction loads
    STRESS_ADDI_BEAM_BOT, // Maximum stress in steel beam bottom Flange due to additional dead loads
    STRESS_ADDI_BEAM_WEB, // Maximum stress in steel beam web due to additional dead loads
    STRESS_ADDI_BEAM_TOP, // Maximum stress in steel beam top Flange  due to additional dead loads
    STRESS_ADDI_CONC_STRESS, // Maximum stress in concrete slab due to additional dead loads
    STRESS_ADDI_CONC_STRAIN, // Maximum strain in concrete slab due to additional dead loads
    STRESS_FINA_LIVE_BEAM_BOT, // Maximum stress in steel beam bottom Flange due to Final stage live dead loads
    STRESS_FINA_LIVE_BEAM_WEB, // Maximum stress in steel beam web due to Final stage live dead loads
    STRESS_FINA_LIVE_BEAM_TOP, // Maximum stress in steel beam top Flange  due to Final stage live dead loads
    STRESS_FINA_LIVE_CONC_STRESS, // Maximum stress in concrete slab due to Final stage live loads
    STRESS_FINA_LIVE_CONC_STRAIN, // Maximum strain in concrete slab due to Final stage live loads
    STRESS_SHRINK_BEAM_BOT, // Maximum stress in steel beam bottom Flange due to shrinkage
    STRESS_SHRINK_BEAM_WEB, // Maximum stress in steel beam web due to shrinkage
    STRESS_SHRINK_BEAM_TOP, // Maximum stress in steel beam top Flange due to shrinkage
    STRESS_SHRINK_CONC_STRESS, // Maximum stress in concrete slab due to shrinkage
    STRESS_SHRINK_CONC_STRAIN, // Maximum strain in concrete slab due to shrinkage
    STRESS_FINA_TOTL_BEAM_BOT, // Maximum stress in steel beam bottom Flange in Final stage
    STRESS_FINA_TOTL_BEAM_WEB, // Maximum stress in steel beam web in Final stage
    STRESS_FINA_TOTL_BEAM_TOP, // Maximum stress in steel beam top Flange in Final stage
    STRESS_FINA_TOTL_CONC_STRESS, // Maximum stress in concrete slab in Final stage
    STRESS_FINA_TOTL_CONC_STRAIN, // Maximum strain in concrete slab in Final stage
  }
}
