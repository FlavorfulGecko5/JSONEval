interface Operand {}

interface VariableOperand : Operand {}

interface PrimitiveOperand : Operand
{
    public PrimitiveOperand Add(PrimitiveOperand b);

    public PrimitiveOperand UnaryAdd();

    public PrimitiveOperand Sub(PrimitiveOperand b);

    public PrimitiveOperand UnarySub();

    public PrimitiveOperand Mult(PrimitiveOperand b);

    public PrimitiveOperand Div(PrimitiveOperand b);

    public PrimitiveOperand Rem(PrimitiveOperand b);
}