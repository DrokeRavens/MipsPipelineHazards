namespace Anthem{
    public enum InstructionType :  byte
    {
        // 0 = R, 1 = I, 2 = J
        LB =  1,
        LH =  1,
        LWL = 1,
        LW =  1,
        LBU = 1,
        LHU = 1,
        LWR = 1,
        SB =  1,
        SH =  1,
        SWL = 1,
        SW =  1,
        SWR = 1,
        ADD  = 0,
        ADDU = 0,
        SUB  = 0,
        SUBU = 0,
        AND  = 0,
        OR   = 0,
        XOR  = 0,
        NOR  = 0,
        SLT  = 0,
        SLTU = 0,
        ADDI = 1,
        ADDIU= 1,
        SLTI = 1,
        SLTIU= 1,
        ANDI = 1,
        ORI  = 1,
        XORI = 1,
        LUI = 1,
        SLL = 0,
        SRL = 0,
        SRA = 0,
        SLLV = 0,
        SRLV = 0,
        SRAV = 0,
        MFHI = 0,
        MTHI = 0,
        MFLO = 0,
        MTLO = 0,
        MULT = 0,
        MULTU = 0,
        DIV = 0,
        DIVU = 0,
        JR = 0,
        JALR = 0,
        BLTZ = 1,
        BGEZ = 1,
        BLTZAL = 1,
        BGEZAL = 1,
        J = 2,
        JAL = 2,
        BEQ    = 1,
        BNE    = 1,
        BLEZ   = 1,
        BGTZ   = 1
    }
}
